using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : ViewBase
{
    [SerializeField] private List<SkillButton> Buttons_SkillCategory;

    [SerializeField] private Transform Parent_Left;
    [SerializeField] private Transform Parent_Right;
    [SerializeField] private GameObject Prefab_SkillSlot;

    private CharacterViewModel _characterVM => ServiceManager.Inst.GetCharacterViewModel();
    private SkillCategory _currentCategory = SkillCategory.None;

    private List<SkillSlot> _leftSlots = new List<SkillSlot>();
    private List<SkillSlot> _rightSlots = new List<SkillSlot>();

    public int CheckCount { get; private set; } = 1;

    private void Awake()
    {
        InitButtons();
        CreateSkillSlot();
    }

    private void OnEnable()
    {
        if (_leftSlots.Count == 0)
        {
            return;
        }

        RefreshCategory();
        SetDefaultCategory();
        SwitchCategory(_currentCategory);
    }

    private void InitButtons()
    {
        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            Buttons_SkillCategory[i].Init(OnClickCategory);
        }
    }

    private void RefreshCategory()
    {
        for (int i = 0; i < Buttons_SkillCategory.Count; i++)
        {
            Buttons_SkillCategory[i].SetCategoryIcon().Forget();
        }
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
            int index = i + 1;
            slot.InitSlot((isOn) => OnClickSkillToggle(_currentCategory, index, isOn));
            _leftSlots.Add(slot);
        }

        for (int i = 0; i < 6; i++)
        {
            SkillSlot slot = Instantiate(Prefab_SkillSlot, Parent_Right).GetComponent<SkillSlot>();
            int index = i + (_leftSlots.Count + 1);
            slot.InitSlot((isOn) => OnClickSkillToggle(_currentCategory, index, isOn));
            _rightSlots.Add(slot);
        }
    }

    private void OnClickSkillToggle(SkillCategory category, int index, bool isOn)
    {
        string skillID = $"Skill_{category}_{index}";

        if (isOn == true)
        {
            _characterVM.GetSkill(skillID);
        }

        UpdateSlots();
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
        RefreshCategory();

        bool isNewGame = SaveManager.Inst.PlayerModel.IsNewGame;

        for (int i = 0; i < _leftSlots.Count; i++)
        {
            string skillID = $"Skill_{_currentCategory}_{i + 1}";
            var data = GameDataManager.Inst.GetData<Skill>(skillID);
            string skillName = data.Name;

            bool canCheck = data.CanGet;
            bool isAbandon = CheckAbandon(skillID);
            bool isSelected = CheckSelect(_currentCategory, skillID);

            bool canInteract;
            if (_currentCategory == SkillCategory.Nature)
            {
                canInteract = isNewGame;
            }
            else
            {
                canInteract = canCheck && !isAbandon;
            }

            _leftSlots[i].SetSlot(skillName, isSelected, canInteract);
        }

        for (int i = 0; i < _rightSlots.Count; i++)
        {
            string skillID = $"Skill_{_currentCategory}_{i + (_leftSlots.Count + 1)}";
            var data = GameDataManager.Inst.GetData<Skill>(skillID);
            string skillName = data.Name;

            bool canCheck = data.CanGet;
            bool isAbandon = CheckAbandon(skillID);
            bool isSelected = CheckSelect(_currentCategory, skillID);

            bool canInteract;
            if (_currentCategory == SkillCategory.Nature)
            {
                canInteract = isNewGame;
            }
            else
            {
                canInteract = canCheck && !isAbandon;
            }

            _rightSlots[i].SetSlot(skillName, isSelected, canInteract);
        }
    }

    private bool CheckSelect(SkillCategory category, string skillID)
    {
        if (_characterVM == null)
        {
            return false;
        }

        return _characterVM.CheckSkill(category, skillID);
    }

    private bool CheckAbandon(string skillID)
    {
        if (_characterVM == null)
        {
            return false;
        }

        return _characterVM.CheckAbandonSkill(skillID);
    }
}
