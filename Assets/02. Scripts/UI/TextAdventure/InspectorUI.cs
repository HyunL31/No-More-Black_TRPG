using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : ViewBase
{
    [SerializeField] private Image Image_Background;
    [SerializeField] private RectTransform Rect_Panorama;
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;
    [SerializeField] private Transform Transform_CharacterContainer;

    private void Awake()
    {
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
    }

    private void OnClickLeft()
    {

    }

    private void OnClickRight()
    {

    }
}
