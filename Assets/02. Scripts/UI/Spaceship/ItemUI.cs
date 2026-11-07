using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : ViewBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Image Image_Item;
    [SerializeField] private TextMeshProUGUI Text_Name;
    [SerializeField] private TextMeshProUGUI Text_Description;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        UIManager.Inst.CloseItemUI();
    }

    public async UniTask SetItemInfo(string itemID)
    {
        Image_Item.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Icon/{itemID}");

        var data = GameDataManager.Inst.GetData<Item>(itemID);

        Text_Name.text = data.Name;
        Text_Description.text = data.Description;
    }
}
