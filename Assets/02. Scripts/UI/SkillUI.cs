using Cysharp.Threading.Tasks.Triggers;
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
    private SkillCategory _currentCategory = SkillCategory.Major;

    private List<SkillSlot> _leftSlots = new List<SkillSlot>();
    private List<SkillSlot> _rightSlots = new List<SkillSlot>();

    private void Start()
    {
        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.CharacterViewModel;
        }

        Init();
    }

    public void Init()
    {
        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            SkillButton button = Buttons_SkillCategory[i];
            button.Init(OnClickCategory);
        }

        CreateSkillSlot();
        SwitchCategory(_currentCategory);
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
        for (int i = 1; i <= _leftSlots.Count; i++)
        {
            Debug.Log(GameDataManager.Inst.GetData<Skill>($"Skill_{_currentCategory}_{i}").Name);
            string skillName = GameDataManager.Inst.GetData<Skill>($"Skill_{_currentCategory}_{i}").Name;
            _leftSlots[i].SetSlot(skillName, CheckSelect(_currentCategory, i));
        }

        for (int i = 1; i <= _rightSlots.Count; i++)
        {
            string skillName = GameDataManager.Inst.GetData<Skill>($"Skill_{_currentCategory}_{i + 6}").Name;
            _leftSlots[i].SetSlot(skillName, CheckSelect(_currentCategory, i + 6));
        }
    }

    private bool CheckSelect(SkillCategory category, int index)
    {
        if (_characterVM == null)
        {
            Debug.LogWarning("CharacterViewModel is null in SkillUI!");
            return false;
        }

        return _characterVM.CheckSkill(category, index);
    }

    private bool CheckUnlock(SkillCategory category)
    {
        return false;
    }
}
