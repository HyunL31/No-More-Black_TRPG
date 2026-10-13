using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Button Button_Category;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Button;
    [SerializeField] private Sprite Sprite_Select;
    [SerializeField] private Sprite Sprite_Deselect;
    [SerializeField] public SkillCategory SkillCategory;

    public bool IsUnlock { get; private set; }

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

    public async UniTask SetCategoryIcon()
    {
        bool isUnlock = SaveManager.Inst.PlayerModel.Characters[0].CategoryUnlock[SkillCategory];

        if (isUnlock)
        {
            // Image_Button.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Icon/{SkillCategory}");
            Button_Category.interactable = true;
            IsUnlock = true;
        }
        else
        {
            Image_Button.sprite = await ResourceManager.Inst.LoadSprite("Sprite/Icon/Unlock");
            Button_Category.interactable = false;
            IsUnlock = false;
        }
    }

    public void SetSelect(bool isSelected)
    {
        // Image_Frame.sprite = isSelected ? Sprite_Select : Sprite_Deselect;

        Image_Frame.color = isSelected ? Color.red : Color.green;
    }
}
