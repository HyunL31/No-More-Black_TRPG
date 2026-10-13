using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static Character;

public class SkillUI : ViewBase
{
    [SerializeField] private List<SkillButton> Buttons_SkillCategory;

    [SerializeField] private Transform Parent_Left;
    [SerializeField] private Transform Parent_Right;
    [SerializeField] private GameObject Prefab_SkillSlot;

    private CharacterViewModel _characterVM;
    private SkillCategory _currentCategory = SkillCategory.None;

    private List<SkillSlot> _leftSlots = new List<SkillSlot>();
    private List<SkillSlot> _rightSlots = new List<SkillSlot>();

    public int CheckCount { get; private set; } = 1;

    private void Start()
    {
        _characterVM = ServiceManager.Inst.CharacterViewModel;

        Init();
    }

    public void Init()
    {
        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.CharacterViewModel;
        }

        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            SkillButton button = Buttons_SkillCategory[i];
            button.Init(OnClickCategory);
            button.SetCategoryIcon().Forget();
        }

        SetDefaultCategory();
        CreateSkillSlot();
        SwitchCategory(_currentCategory);
    }

    private void SetDefaultCategory()
    {
        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            SkillButton button = Buttons_SkillCategory[i];

            if (button.IsUnlock == true)
            {
                _currentCategory = button.SkillCategory;
                return;
            }
        }
    }

    private void CreateSkillSlot()
    {
        for (int i = 0; i < 6; i++)
        {
            SkillSlot slot = Instantiate(Prefab_SkillSlot, Parent_Left).GetComponent<SkillSlot>();
            _leftSlots.Add(slot);
        }

        for (int i = 0; i < 6; i++)
        {
            SkillSlot slot = Instantiate(Prefab_SkillSlot, Parent_Right).GetComponent<SkillSlot>();
            _rightSlots.Add(slot);
        }
    }

    private void OnClickCategory(SkillCategory category)
    {
        SwitchCategory(category);
    }

    private void SwitchCategory(SkillCategory skillCategory)
    {
        _currentCategory = skillCategory;

        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            bool isSelected = (Buttons_SkillCategory[i].SkillCategory == skillCategory);
            Buttons_SkillCategory[i].SetSelect(isSelected);
        }

        UpdateSlots();
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < _leftSlots.Count; i++)
        {
            var data = GameDataManager.Inst.GetData<Skill>($"Skill_{_currentCategory}_{i + 1}");
            string skillName = data.Name;
            bool canCheck = data.CanGet;

            _leftSlots[i].SetSlot(skillName, CheckSelect(_currentCategory, i + 1));
            _leftSlots[i].CheckInteractable(canCheck);
        }

        for (int i = 0; i < _rightSlots.Count; i++)
        {
            var data = GameDataManager.Inst.GetData<Skill>($"Skill_{_currentCategory}_{i + (_leftSlots.Count + 1)}");
            string skillName = data.Name;
            bool canCheck = data.CanGet;

            _rightSlots[i].SetSlot(skillName, CheckSelect(_currentCategory, i + (_leftSlots.Count + 1)));
            _rightSlots[i].CheckInteractable(canCheck);
        }
    }

    private bool CheckSelect(SkillCategory category, int index)
    {
        if (_characterVM == null)
        {
            return false;
        }

        return _characterVM.CheckSkill(category, index);
    }
}
