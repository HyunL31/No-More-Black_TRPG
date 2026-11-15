using UnityEngine;
using UnityEngine.UI;

public class MainUI : ViewBase
{
    [SerializeField] private Button Button_Inventory;
    [SerializeField] private Minimap Minimap;

    private void Awake()
    {
        Button_Inventory.onClick.AddListener(OnClickInventory);
    }

    private void OnClickInventory()
    {
        UIManager.Inst.OpenInventoryUI();
    }

    public Minimap GetMinimap()
    {
        return Minimap;
    }
}
