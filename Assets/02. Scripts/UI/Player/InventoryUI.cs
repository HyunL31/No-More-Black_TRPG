using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : ViewBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_Character;

    [SerializeField] private Transform Transform_Character;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private ItemSlot Prefab_ItemSlot;

    [SerializeField] private TextMeshProUGUI Text_ItemName;
    [SerializeField] private TextMeshProUGUI Text_ItemDescription;

    private ObjectPooling<ItemSlot> _itemPool;
    private readonly List<ItemSlot> _activeSlots = new List<ItemSlot>();

    private Dictionary<string, CharacterState> _characterState = new Dictionary<string, CharacterState>();

    public string CharacterID { get; private set; } = "Elysia_01";

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
        Button_Character.onClick.AddListener(OnClickCharacter);

        _itemPool = new ObjectPooling<ItemSlot>(Prefab_ItemSlot, Transform_SlotParent, 3, 10);
    }

    private void OnEnable()
    {
        Text_ItemName.text = "";
        Text_ItemDescription.text = "";

        SpawnCharacter().Forget();
        InitSlot();
    }

    private void OnClickClose()
    {
        UIManager.Inst.CloseInventoryUI();
    }

    private void OnClickCharacter()
    {
        SetCharacterBasic();
    }

    private void SetCharacter(string characterID)
    {
        CharacterID = characterID;

        foreach (CharacterState character in _characterState.Values)
        {
            character.gameObject.SetActive(false);
        }
    }

    private void InitSlot()
    {
        ClearSaveSlot();

        Dictionary<string, int> inventory = SaveManager.Inst.PlayerModel.Inventory;

        foreach (string item in inventory.Keys)
        {
            ItemSlot itemSlot = _itemPool.Get();
            _activeSlots.Add(itemSlot);

            itemSlot.SetItemSlot(item).Forget();
            itemSlot.InitAction((itemSlot, itemID) => SetItemDescription(itemSlot, itemID));
        }
    }

    private void SetItemDescription(ItemSlot slot, string itemID)
    {
        ClearSlotState(slot);

        var data = GameDataManager.Inst.GetData<Item>(itemID);

        Text_ItemName.text = data.Name;
        Text_ItemDescription.text = data.Description;
    }

    private void ClearSlotState(ItemSlot slot)
    {
        foreach (ItemSlot itemSlot in _activeSlots)
        {
            itemSlot.SetSelected(itemSlot == slot);
        }
    }

    private async UniTask SpawnCharacter()
    {
        if (_characterState.ContainsKey(CharacterID) == true)
        {
            _characterState[CharacterID].gameObject.SetActive(true);
        }
        else
        {
            GameObject prefab = await ResourceManager.Inst.InstantiateAsync($"Spine/{CharacterID}", Transform_Character);
            CharacterState characterState = prefab.GetComponent<CharacterState>();

            _characterState.Add(CharacterID, characterState);
        }

        _characterState[CharacterID].ApplyEmotion("basic", "smile");
    }

    private void SetCharacterBasic()
    {
        _characterState[CharacterID].PlayAnimOnce("active", "basic", "smile");
    }

    private void ClearSaveSlot()
    {
        foreach (ItemSlot slot in _activeSlots)
        {
            _itemPool.Release(slot);
        }

        _activeSlots.Clear();
    }
}
