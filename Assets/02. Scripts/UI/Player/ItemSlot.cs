using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image Image_Background;
    [SerializeField] private Button Button_Item;

    private Color _selectColor = Color.red;
    private Color _normalColor = Color.blue;

    private void Awake()
    {
        Button_Item.onClick.AddListener(OnClickItem);
    }

    private void OnClickItem()
    {
        
    }
}