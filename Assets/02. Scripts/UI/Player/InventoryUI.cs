using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : ViewBase
{
    [SerializeField] private Button Button_Close;

    [SerializeField] private Transform Transform_Character;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_ItemSlot;

    [SerializeField] private TextMeshProUGUI Text_ItemName;
    [SerializeField] private TextMeshProUGUI Text_ItemDescription;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        UIManager.Inst.CloseInventoryUI();
    }
}
