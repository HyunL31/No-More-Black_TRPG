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
    [SerializeField] private Button Button_Confirm;

    private CharacterViewModel _characterVM => ServiceManager.Inst.GetCharacterViewModel();
    private SkillCategory _currentCategory = SkillCategory.None;

    private List<SkillSlot> _leftSlots = new List<SkillSlot>();
    private List<SkillSlot> _rightSlots = new List<SkillSlot>();

    private string _natureSkill = "";
    private string _abandonSkill = "";

    public bool IsAbandon { get; private set; } = false;
    public int CheckCount { get; private set; } = 1;

    private void Awake()
    {
        InitButtons();
        CreateSkillSlot();

        Button_Resume.onClick.AddListener(OnClickResume);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnEnable()
    {
        if (SaveManager.Inst.PlayerModel.IsNewGame == true)
        {
            Button_Resume.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(true);
        }
        else
        {
            Button_Resume.gameObject.SetActive(true);
            Button_Confirm.gameObject.SetActive(false);
        }

        if (IsAbandon == true)
        {
            Button_Resume.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(true);
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

    private void OnClickConfirm()
    {
        if (IsAbandon == false)
        {
            if (_natureSkill == "")
            {
                return;
            }

            AddNatureSkill();

            UIManager.Inst.OpenVisualNovelUI();
            UIManager.Inst.OpenDialogueUI();
        }
        else
        {
            if (_abandonSkill == "")
            {
                return;
            }

            _characterVM.AbandonSkill(_abandonSkill);
            _abandonSkill = "";

            SetAbandonUI(false);
            // TODO: 다음날로 넘어가기 등
        }

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

        if (IsAbandon == true)
        {
            if (isOn == false)
            {
                _abandonSkill = skillID;
            }
            else if (_abandonSkill == skillID)
            {
                _abandonSkill = "";
            }

            UpdateSlots();
            return;
        }

        if (isOn == true)
        {
            _natureSkill = skillID;
        }
        else if (_natureSkill == skillID)
        {
            _natureSkill = "";
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

        for (int i = 0; i < _leftSlots.Count; i++)
        {
            UpdateSlot(_leftSlots[i], i + 1);
        }

        for (int i = 0; i < _rightSlots.Count; i++)
        {
            UpdateSlot(_rightSlots[i], i + _leftSlots.Count + 1);
        }
    }

    private void UpdateSlot(SkillSlot slot, int index)
    {
        string skillID = $"Skill_{_currentCategory}_{index}";

        var data = GameDataManager.Inst.GetData<Skill>(skillID);

        bool hasSkill = CheckSelect(_currentCategory, skillID);
        bool isAbandoned = CheckAbandon(skillID);

        bool canInteract;
        bool isSelected;

        if (IsAbandon == true)
        {
            canInteract = hasSkill && !isAbandoned;
            isSelected = hasSkill && !isAbandoned && _abandonSkill != skillID;
        }
        else if (_currentCategory == SkillCategory.Nature)
        {
            canInteract = SaveManager.Inst.PlayerModel.IsNewGame;
            isSelected = _natureSkill == skillID;
        }
        else
        {
            canInteract = data.CanGet && !isAbandoned;
            isSelected = hasSkill;
        }

        slot.SetSlot(data.Name, isSelected, canInteract);
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

    public void SetAbandonUI(bool isAbandon)
    {
        IsAbandon = isAbandon;
        _abandonSkill = "";

        if (_leftSlots.Count > 0)
        {
            UpdateSlots();
        }
    }
}
