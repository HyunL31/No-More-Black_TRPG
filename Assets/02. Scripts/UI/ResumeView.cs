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

    private GameObject[] _staminaSlots;
    private GameObject[] _crazySlots;
    private GameObject[] _oblivionSlots;

    private CharacterViewModel _characterViewModel;
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
        if (_characterViewModel != null)
        {
            Refresh();
        }
    }

    private void Start()
    {
        _characterViewModel = ServiceManager.Inst.GetCharacterViewModel();
        _characterViewModel.PropertyChanged += OnPropertyChanged;

        _staminaSlots = InitializeGaugeSlot(Parent_Stamina, MaxGaugeCount);
        _crazySlots = InitializeGaugeSlot(Parent_Crazy, MaxGaugeCount);
        _oblivionSlots = InitializeGaugeSlot(Parent_Oblivion, MaxGaugeCount);

        Refresh();
    }

    private void OnDestroy()
    {
        if (_characterViewModel != null)
        {
            _characterViewModel.PropertyChanged -= OnPropertyChanged;
        }
    }

    private void Refresh()
    {
        _data = GameDataManager.Inst.GetData<Character>(_characterViewModel.CurrentCharacter);
        _relations = _data.GetRelations();
        _relationIndex = 0;

        Debug.Log($"ID:{_data.ID} 관계수:{_relations.Count} 체력:{_characterViewModel.Stamina} 열광:{_characterViewModel.Crazy} 망각:{_characterViewModel.Oblivion}");

        SetCharacterData().Forget();
        UpdateAllGauges();
        RefreshRelationshipUI().Forget();
    }

    private GameObject[] InitializeGaugeSlot(Transform parent, int maxCount)
    {
        GameObject[] slots = new GameObject[maxCount];

        while (parent.childCount < maxCount)
        {
            GameObject prefab = Instantiate(Slot_Guage, parent);
            
        }
    }

    private void UpdateAllGauges()
    {
        UpdateGaugeSlots(Parent_Stamina, _characterViewModel.Stamina);
        UpdateGaugeSlots(Parent_Crazy, _characterViewModel.Crazy);
        UpdateGaugeSlots(Parent_Oblivion, _characterViewModel.Oblivion);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CharacterViewModel.Stamina):
                UpdateGaugeSlots(Parent_Stamina, _characterViewModel.Stamina);
                break;

            case nameof(CharacterViewModel.Crazy):
                UpdateGaugeSlots(Parent_Crazy, _characterViewModel.Crazy);
                break;

            case nameof(CharacterViewModel.Oblivion):
                UpdateGaugeSlots(Parent_Oblivion, _characterViewModel.Oblivion);
                break;

            case nameof(CharacterViewModel.Ideology):
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

    private void UpdateGaugeSlots(Transform parent, int currentValue)
    {
        if (parent == null || Slot_Guage == null)
        {
            return;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(i < currentValue);
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
}