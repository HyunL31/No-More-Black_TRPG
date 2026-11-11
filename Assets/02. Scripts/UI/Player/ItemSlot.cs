using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image Image_Background;
    [SerializeField] private Button Button_Item;
    [SerializeField] private TextMeshProUGUI Text_Count;

    private Color _selectColor = Color.red;
    private Color _normalColor = Color.blue;

    private string _itemID;
    private bool _isSelected = false;

    private Action<ItemSlot, string> _onChangeSlot;

    private void Awake()
    {
        Button_Item.onClick.AddListener(OnClickItem);
    }

    private void OnEnable()
    {
        _isSelected = false;
        ChangeColor();
    }

    private void OnClickItem()
    {
        _isSelected = !_isSelected;
        ChangeColor();

        _onChangeSlot?.Invoke(this, _itemID);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        ChangeColor();
    }

    public void ChangeColor()
    {
        if (_isSelected == true)
        {
            Image_Background.color = _selectColor;
        }
        else
        {
            Image_Background.color = _normalColor;
        }
    }

    public async UniTask SetItemSlot(string itemID)
    {
        _itemID = itemID;

        Button_Item.image.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Icon/{_itemID}");
        Text_Count.text = SaveManager.Inst.PlayerModel.Inventory[itemID].ToString();
    }

    public void InitAction(Action<ItemSlot, string> onChangeSlot)
    {
        _onChangeSlot -= onChangeSlot;
        _onChangeSlot += onChangeSlot;
    }
}