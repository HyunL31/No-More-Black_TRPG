using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : ViewBase
{
    [SerializeField] private List<SkillButton> Buttons_SkillCategory;

    [SerializeField] private Transform Parent_Left;
    [SerializeField] private Transform Parent_Right;
    [SerializeField] private GameObject Prefab_SkillSlot;
    [SerializeField] private Button Button_Resume;
    [SerializeField] private Button Button_NatureConfirm;

    private CharacterViewModel _characterVM => ServiceManager.Inst.GetCharacterViewModel();
    private SkillCategory _currentCategory = SkillCategory.None;

    private List<SkillSlot> _leftSlots = new List<SkillSlot>();
    private List<SkillSlot> _rightSlots = new List<SkillSlot>();

    private string _natureSkill = "";

    public int CheckCount { get; private set; } = 1;

    private void Awake()
    {
        InitButtons();
        CreateSkillSlot();

        Button_Resume.onClick.AddListener(OnClickResume);
        Button_NatureConfirm.onClick.AddListener(OnClickNatureConfirm);
    }

    private void OnEnable()
    {
        if (SaveManager.Inst.PlayerModel.IsNewGame == true)
        {
            Button_Resume.gameObject.SetActive(false);
            Button_NatureConfirm.gameObject.SetActive(true);
        }
        else
        {
            Button_Resume.gameObject.SetActive(true);
            Button_NatureConfirm.gameObject.SetActive(false);
        }

        if (_leftSlots.Count == 0)
        {
            return;
        }

        _characterVM.SetCurrentCharacter("Player_01");

        RefreshCategory();
        SetDefaultCategory();
        SwitchCategory(_currentCategory);
    }

    private void OnClickResume()
    {
        UIManager.Inst.OpenResumeUI();
        UIManager.Inst.CloseSkillUI();
    }

    private void OnClickNatureConfirm()
    {
        if (_natureSkill == "")
        {
            return;
        }

        AddNatureSkill();

        UIManager.Inst.OpenVisualNovelUI();
        UIManager.Inst.OpenDialogueUI();

        UIManager.Inst.CloseSkillUI();
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
            _natureSkill = skillID;
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
            bool isSelected = (_currentCategory == SkillCategory.Nature && skillID == _natureSkill) || CheckSelect(_currentCategory, skillID);

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
            bool isSelected = (_currentCategory == SkillCategory.Nature && skillID == _natureSkill) || CheckSelect(_currentCategory, skillID);

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

    private void AddNatureSkill()
    {
        _characterVM.GetSkill(_natureSkill);

        SaveManager.Inst.PlayerModel.IsNewGame = false;
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
