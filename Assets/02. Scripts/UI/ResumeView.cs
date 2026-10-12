using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeView : ViewBase
{
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
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCName;
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCRelationship;

    private const int MaxGaugeCount = 10;

    private GuageSlot[] _staminaSlots;
    private GuageSlot[] _crazySlots;
    private GuageSlot[] _oblivionSlots;

    private CharacterViewModel _characterVM;
    private Character _data;
    private List<Relation> _relations = new List<Relation>();
    private int _relationIndex = 0;

    private void Awake()
    {
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
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

        Refresh();
    }

    private void OnDestroy()
    {
        if (_characterVM != null)
        {
            _characterVM.PropertyChanged -= OnPropertyChanged;
        }
    }

    private void Refresh()
    {
        _data = GameDataManager.Inst.GetData<Character>(_characterVM.CurrentCharacter);
        _relations = _data.GetRelations();
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

        Relation prev = _relations[(_relationIndex - 1 + total) % total];
        Relation current = _relations[_relationIndex];
        Relation next = _relations[(_relationIndex + 1) % total];

        Character currentData = GameDataManager.Inst.GetData<Character>(current.TargetID);

        if (Text_CurrentNPCName != null)
        {
            Text_CurrentNPCName.text = currentData != null ? currentData.Name : current.TargetID;
        }

        if (Text_CurrentNPCRelationship != null)
        {
            Text_CurrentNPCRelationship.text = GetRelationText(current.Type);
        }

        await SetImage(Image_PrevNPC, prev.TargetID);
        await SetImage(Image_CurrentNPC, current.TargetID);
        await SetImage(Image_NextNPC, next.TargetID);
    }

    private async UniTask SetImage(Image image, string characterID)
    {
        Sprite sprite = await ResourceManager.Inst.LoadAsset<Sprite>($"Sprites/Character/{characterID}");

        if (image != null && sprite != null)
        {
            image.sprite = sprite;
        }
    }

    private void OnClickLeft()
    {
        int total = _relations.Count;

        if (total == 0)
        {
            return;
        }

        _relationIndex = (_relationIndex - 1 + total) % total;
        RefreshRelationshipUI().Forget();
    }

    private void OnClickRight()
    {
        int total = _relations.Count;

        if (total == 0)
        {
            return;
        }

        _relationIndex = (_relationIndex + 1) % total;
        RefreshRelationshipUI().Forget();
    }

    private string GetRelationText(RelationType relation)
    {
        switch (relation)
        {
            case RelationType.Hostile: return "적대";
            case RelationType.Friction: return "불화";
            case RelationType.Neutral: return "중립";
            case RelationType.Friendly: return "우호";
            case RelationType.Friendship: return "우정";
            case RelationType.Pact: return "맹약";
            default: return "중립";
        }
    }

    private void UpdateIdeology(float ideology)
    {
        float normalizedVal = Mathf.Clamp(ideology / 3f, -1f, 1f);

        Image_LeftIdeology.fillAmount = normalizedVal < 0 ? Mathf.Abs(normalizedVal) : 0f;
        Image_RightIdeology.fillAmount = normalizedVal > 0 ? normalizedVal : 0f;

        Text_Ideology.text = GetIdeologyName(ideology);
    }

    private string GetIdeologyName(float ideology)
    {
        if (ideology == 0)
        {
            return "균형";
        }
        else
        {
            if (ideology <= -3)
            {
                return "혁명";
            }
            else if (ideology == -2)
            {
                return "개혁";
            }
            else if (ideology == -1)
            {
                return "발전";
            }
            else if (ideology == 1)
            {
                return "안정";
            }
            else if (ideology == 2)
            {
                return "헌신";
            }
            else
            {
                return "수호";
            }
        }
    }
}