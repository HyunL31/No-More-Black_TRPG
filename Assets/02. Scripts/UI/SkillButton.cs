using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Button Button_Category;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Sprite Sprite_Select;
    [SerializeField] private Sprite Sprite_Deselect;
    [SerializeField] public SkillCategory SkillCategory;

    private Action<SkillCategory> _onClickButton;

    public void Init(Action<SkillCategory> onClickCategory)
    {
        _onClickButton = onClickCategory;

        Button_Category.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        _onClickButton?.Invoke(SkillCategory);
    }

    public void SetSelect(bool isSelected)
    {
        //Image_Frame.sprite = isSelected ? Sprite_Select : Sprite_Deselect;

        Image_Frame.color = isSelected ? Color.red : Color.green;
    }
}
