using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeView : ViewBase
{
    private struct SlotState
    {
        public Vector2 Pos;
        public Vector3 Scale;
        public Color Color;
    }

    [Header("Player")]
    [SerializeField] private Image Image_Character;
    [SerializeField] private TextMeshProUGUI Text_RegisterCode;
    [SerializeField] private TextMeshProUGUI Text_Job;
    [SerializeField] private TextMeshProUGUI Text_CharacterName;

    [Header("Operation")]
    [SerializeField] private GameObject Slot_Guage;
    [SerializeField] private Transform Parent_Stamina;
    [SerializeField] private Transform Parent_Crazy;
    [SerializeField] private Transform Parent_Oblivion;

    [Header("Ideology")]
    [SerializeField] private Image Image_LeftIdeology;
    [SerializeField] private Image Image_RightIdeology;
    [SerializeField] private TextMeshProUGUI Text_Ideology;

    [Header("Relationship")]
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;

    [SerializeField] private Image Image_PrevNPC;
    [SerializeField] private Image Image_NextNPC;
    [SerializeField] private Image Image_CurrentNPC;
    [SerializeField] private Image Image_OutNext;
    [SerializeField] private Image Image_OutPrev;
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCName;
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCRelationship;

    [SerializeField] private Button Button_Skill;

    private const int MaxGaugeCount = 10;
    private const int SlotCount = 5;
    private const int CurrentSlot = 2;

    private GuageSlot[] _staminaSlots;
    private GuageSlot[] _crazySlots;
    private GuageSlot[] _oblivionSlots;

    private CharacterViewModel _characterVM;
    private Character _data;
    private List<Relation> _relations = new List<Relation>();
    private int _relationIndex = 0;

    private float _slideDuration = 0.3f;

    private Image[] _slotImage;
    private SlotState[] _slotState;
    private Sequence _slideSeq;
    private bool _isAnimating;

    private void Awake()
    {
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
        Button_Skill.onClick.AddListener(OnClickSkill);
    }

    private void OnEnable()
    {
        if (_characterVM != null)
        {
            Refresh();
        }
    }

    private void Start()
    {
        _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        _characterVM.PropertyChanged += OnPropertyChanged;

        _staminaSlots = InitializeGaugeSlot(Parent_Stamina, MaxGaugeCount);
        _crazySlots = InitializeGaugeSlot(Parent_Crazy, MaxGaugeCount);
        _oblivionSlots = InitializeGaugeSlot(Parent_Oblivion, MaxGaugeCount);

        SetSlotState();
        Refresh();
    }

    private void OnDisable()
    {
        _slideSeq?.Kill();

        ResetSlot();
        _isAnimating = false;
    }

    private void OnDestroy()
    {
        _slideSeq?.Kill();

        if (_characterVM != null)
        {
            _characterVM.PropertyChanged -= OnPropertyChanged;
        }
    }

    private void OnClickSkill()
    {
        UIManager.Inst.OpenSkillUI();
        UIManager.Inst.CloseResumeUI();
    }

    private void Refresh()
    {
        _data = GameDataManager.Inst.GetData<Character>(_characterVM.CurrentCharacter);

        _relations = _characterVM.CurrentCharacterModel.Relations;
        _relations.Sort((a, b) => a.Type.CompareTo(b.Type));

        _relationIndex = 0;

        Debug.Log($"ID:{_data.ID} 관계수:{_relations.Count} 체력:{_characterVM.Stamina} 열광:{_characterVM.Crazy} 망각:{_characterVM.Oblivion} 이념:{_characterVM.Ideology}");

        SetCharacterData().Forget();
        UpdateAllGauges();
        RefreshRelationshipUI().Forget();
    }

    private GuageSlot[] InitializeGaugeSlot(Transform parent, int maxCount)
    {
        GuageSlot[] slots = new GuageSlot[maxCount];

        for (int i = 0; i < maxCount; i++)
        {
            GameObject newSlot = Instantiate(Slot_Guage, parent);
            slots[i] = newSlot.GetComponent<GuageSlot>();
        }

        return slots;
    }

    private void UpdateAllGauges()
    {
        UpdateGaugeSlots(_staminaSlots, _characterVM.Stamina, false);
        UpdateGaugeSlots(_crazySlots, _characterVM.Crazy, true);
        UpdateGaugeSlots(_oblivionSlots, _characterVM.Oblivion, true);
        UpdateIdeology(_characterVM.Ideology);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CharacterViewModel.Stamina):
                UpdateGaugeSlots(_staminaSlots, _characterVM.Stamina, false);
                break;

            case nameof(CharacterViewModel.Crazy):
                UpdateGaugeSlots(_crazySlots, _characterVM.Crazy, true);
                break;

            case nameof(CharacterViewModel.Oblivion):
                UpdateGaugeSlots(_oblivionSlots, _characterVM.Oblivion, true);
                break;

            case nameof(CharacterViewModel.Ideology):
                UpdateIdeology(_characterVM.Ideology);
                break;
        }
    }

    private async UniTask SetCharacterData()
    {
        Text_CharacterName.text = _data.Name;
        Text_RegisterCode.text = _data.RegisterCode;
        Text_Job.text = _data.Job;

        await SetImage(Image_Character, _data.ID);
    }

    private void UpdateGaugeSlots(GuageSlot[] slots, int currentValue, bool isInverse = false)
    {
        int totalCount = slots.Length;

        for (int i = 0; i < totalCount; i++)
        {
            if (slots[i] != null)
            {
                bool isActive = i < currentValue;

                float t = totalCount > 1 ? (float)i / (totalCount - 1) : 0f;
                float colorFactor = isInverse ? t : (1f - t);

                Color slotColor = Color.Lerp(Color.blue, Color.red, colorFactor);

                slots[i].SetActiveImage(isActive, slotColor);
            }
        }
    }

    private async UniTask RefreshRelationshipUI()
    {
        int total = _relations.Count;
        if (total == 0)
        {
            return;
        }

        _relationIndex = Mathf.Clamp(_relationIndex, 0, total - 1);

        ApplyRelationText(_relationIndex);

        await UniTask.WhenAll(
            SetImage(Image_OutPrev, GetRelationId(_relationIndex - 2)),
            SetImage(Image_PrevNPC, GetRelationId(_relationIndex - 1)),
            SetImage(Image_CurrentNPC, GetRelationId(_relationIndex)),
            SetImage(Image_NextNPC, GetRelationId(_relationIndex + 1)),
            SetImage(Image_OutNext, GetRelationId(_relationIndex + 2))
        );
    }

    private string GetRelationId(int index)
    {
        int total = _relations.Count;
        return _relations[((index % total) + total) % total].TargetID;
    }

    private void SetSlotState()
    {
        _slotImage = new[] { Image_OutPrev, Image_PrevNPC, Image_CurrentNPC, Image_NextNPC, Image_OutNext };
        _slotState = new SlotState[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            RectTransform rect = _slotImage[i].rectTransform;
            _slotState[i] = new SlotState { Pos = rect.anchoredPosition, Scale = rect.localScale, Color = _slotImage[i].color };
        }
    }

    private void ResetSlot()
    {
        if (_slotImage == null)
        {
            return;
        }

        for (int i = 0; i < SlotCount; i++)
        {
            _slotImage[i].DOKill();
            _slotImage[i].rectTransform.DOKill();

            RectTransform rect = _slotImage[i].rectTransform;
            rect.anchoredPosition = _slotState[i].Pos;
            rect.localScale = _slotState[i].Scale;
            _slotImage[i].color = _slotState[i].Color;
        }
    }

    private async UniTaskVoid MoveRelation(int dir)
    {
        int total = _relations.Count;
        if (total <= 1 || _isAnimating)
        {
            return;
        }

        _isAnimating = true;

        int newIndex = (_relationIndex + dir + total) % total;

        Sprite[] sprites = await UniTask.WhenAll(Enumerable.Range(0, SlotCount).Select(i => ResourceManager.Inst.LoadSprite($"Sprites/Character/{GetRelationId(newIndex + i - CurrentSlot)}")));

        if (!isActiveAndEnabled)
        {
            _isAnimating = false;
            return;
        }

        _slideSeq?.Kill();
        _slideSeq = DOTween.Sequence();

        for (int i = 0; i < SlotCount; i++)
        {
            int targetSlot = i - dir;

            SlotState target = (targetSlot >= 0 && targetSlot < SlotCount) ? _slotState[targetSlot] : new SlotState { Pos = _slotState[i].Pos, Scale = _slotState[i].Scale, Color = new Color(_slotState[i].Color.r, _slotState[i].Color.g, _slotState[i].Color.b, 0f) };

            _slideSeq.Join(_slotImage[i].rectTransform.DOAnchorPos(target.Pos, _slideDuration).SetEase(Ease.OutCubic));
            _slideSeq.Join(_slotImage[i].rectTransform.DOScale(target.Scale, _slideDuration).SetEase(Ease.OutCubic));
            _slideSeq.Join(_slotImage[i].DOColor(target.Color, _slideDuration).SetEase(Ease.OutCubic));
        }

        _slideSeq.InsertCallback(_slideDuration * 0.5f, () => ApplyRelationText(newIndex));

        var tcs = new UniTaskCompletionSource();
        _slideSeq.OnComplete(() => tcs.TrySetResult());
        _slideSeq.OnKill(() => tcs.TrySetResult());
        await tcs.Task;

        if (!isActiveAndEnabled)
        {
            _isAnimating = false;
            return;
        }

        _relationIndex = newIndex;
        ResetSlot();

        for (int i = 0; i < SlotCount; i++)
        {
            if (sprites[i] != null)
            {
                _slotImage[i].sprite = sprites[i];
            }
        }

        ApplyRelationText(newIndex);

        _isAnimating = false;
    }

    private void ApplyRelationText(int index)
    {
        Relation current = _relations[index];
        Character currentData = GameDataManager.Inst.GetData<Character>(current.TargetID);

        Text_CurrentNPCName.text = currentData != null ? currentData.Name : current.TargetID;
        Text_CurrentNPCRelationship.text = GameUtil.GetRelationText(current.Type);
    }

    private async UniTask SetImage(Image image, string characterID)
    {
        Sprite sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{characterID}");

        if (image != null && sprite != null)
        {
            image.sprite = sprite;
        }
    }

    private void OnClickLeft()
    {
        MoveRelation(-1).Forget();
    }

    private void OnClickRight()
    {
        MoveRelation(1).Forget();
    }

    private void UpdateIdeology(float ideology)
    {
        float normalizedVal = Mathf.Clamp(ideology / 3f, -1f, 1f);

        Image_LeftIdeology.fillAmount = normalizedVal < 0 ? Mathf.Abs(normalizedVal) : 0f;
        Image_RightIdeology.fillAmount = normalizedVal > 0 ? normalizedVal : 0f;

        Text_Ideology.text = GameUtil.GetIdeologyName(ideology);
    }
}