using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DiceResult
{
    CriticalSuccess,
    ExtremeSuccess,
    HardSuccess,
    ReasonSuccess,
    Null,
    Fail,
    CriticalFail
}

public class DiceUI : ViewBase
{
    [SerializeField] private DiceEditSlot Prefab_EditInfo;

    [Header("Button")]
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_NormalDice;

    [Header("Opponent")]
    [SerializeField] private TextMeshProUGUI Text_OpponentTitle;
    [SerializeField] private TextMeshProUGUI Text_OpponentDice;
    [SerializeField] private GameObject Panel_SkillText;
    [SerializeField] private TextMeshProUGUI Text_OpponentSkill;
    [SerializeField] private Transform Parent_OpponentBonusEdit;
    [SerializeField] private Transform Parent_OpponentMinusEdit;

    [Header("Player")]
    [SerializeField] private TextMeshProUGUI Text_PlayerDice;
    [SerializeField] private TMP_Dropdown Dropdown_Skill;
    [SerializeField] private Transform Parent_PlayerBonusEdit;
    [SerializeField] private Transform Parent_PlayerMinusEdit;

    private CharacterViewModel _characterVM;

    private DiceController _playerDiceController;
    private DiceController _opponentDiceController;
    private bool _isRolling = false;
    private bool _canClose = false;

    private string _diceID;
    private bool _boost = false;
    private bool _special = false;
    private int _nullCount = 0;
    private int _completeDiceCount = 0;

    private int _playerOriginDice;
    private int _opponentOriginDice;

    private int _playerDice;
    private int _opponentDice;

    private DiceResult _diceResult;
    private List<string> _skillID = new List<string>();

    private List<string> _playerEditDice = new List<string>();
    private List<string> _opponentEditDice = new List<string>();

    private ObjectPooling<DiceEditSlot> _editPool;
    private List<DiceEditSlot> _activeSlot = new List<DiceEditSlot>();

    private List<DiceEditSlot> _playerBonusSlots = new List<DiceEditSlot>();
    private List<DiceEditSlot> _playerMinusSlots = new List<DiceEditSlot>();
    private List<DiceEditSlot> _opponentSlots = new List<DiceEditSlot>();

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
        Button_NormalDice.onClick.AddListener(OnClickDice);

        _characterVM = ServiceManager.Inst.GetCharacterViewModel();

        _editPool = new ObjectPooling<DiceEditSlot>(Prefab_EditInfo, Parent_PlayerBonusEdit, 10, 30);
    }

    private void OnEnable()
    {
        _nullCount = 0;
        _completeDiceCount = 0;
        _special = false;
        _canClose = false;

        Button_NormalDice.gameObject.SetActive(true);

        _isRolling = false;

        Text_PlayerDice.text = "";
        Text_OpponentDice.text = "";

        InitDiceObject();
    }

    private void OnDisable()
    {
        if (_playerDiceController != null)
        {
            _playerDiceController.OnRollComplete -= CompleteDiceRoll;
            _playerDiceController.OnPlusHit -= OnPlayerPlusHit;
            _playerDiceController.OnMinusHit -= OnPlayerMinusHit;
        }

        if (_opponentDiceController != null)
        {
            _opponentDiceController.OnRollComplete -= CompleteDiceRoll;
            _opponentDiceController.OnLandHit -= OnOpponentHit;
        }
    }

    private void OnClickDice()
    {
        RollNormalDice();

        Dropdown_Skill.gameObject.SetActive(false);
        Panel_SkillText.SetActive(false);
    }

    private void OnClickClose()
    {
        if (_canClose == true)
        {
            GameObjectManager.Inst.CloseDice();
            UIManager.Inst.OpenDialogueUI();
            UIManager.Inst.CloseDiceUI();
        }
        else
        {
            return;
        }
    }

    public void SetDiceID(string diceID)
    {
        _diceID = diceID;
        InitDropdown();
        SetDiceEdit();
    }

    public void SetOpponent()
    {
        string opponent = GameDataManager.Inst.GetData<Dice>(_diceID).Opponent;

        if (opponent == "System")
        {
            Text_OpponentTitle.text = "판정치";
            Panel_SkillText.SetActive(false);

            return;
        }

        Panel_SkillText.SetActive(true);

        Text_OpponentTitle.text = $"{GameDataManager.Inst.GetData<Character>(opponent).Name}";
    }

    private void InitDiceObject()
    {
        _playerDiceController = GameObjectManager.Inst.InitPlayerDice();
        _opponentDiceController = GameObjectManager.Inst.InitOpponentDice();

        _playerDiceController.OnRollComplete += CompleteDiceRoll;
        _opponentDiceController.OnRollComplete += CompleteDiceRoll;

        _playerDiceController.OnPlusHit += OnPlayerPlusHit;
        _playerDiceController.OnMinusHit += OnPlayerMinusHit;

        _opponentDiceController.OnLandHit += OnOpponentHit;
    }

    private void InitDropdown()
    {
        _characterVM.SetCurrentCharacter("Player_01");
        _skillID.Clear();

        Dropdown_Skill.gameObject.SetActive(true);
        Dropdown_Skill.ClearOptions();

        string natureSkill = GameDataManager.Inst.GetData<Skill>(_characterVM.CurrentCharacterModel.SelectedNature).Name;
        Dropdown_Skill.options.Add(new TMP_Dropdown.OptionData(natureSkill));
        _skillID.Add(_characterVM.CurrentCharacterModel.SelectedNature);
        
        SetSkillDropdown(_characterVM.CurrentCharacterModel.SelectedMajor);
        SetSkillDropdown(_characterVM.CurrentCharacterModel.SelectedCareer);
        SetSkillDropdown(_characterVM.CurrentCharacterModel.SelectedWisdom);
        SetSkillDropdown(_characterVM.CurrentCharacterModel.SelectedMethod);
        SetSkillDropdown(_characterVM.CurrentCharacterModel.SelectedResistance);

        Dropdown_Skill.value = 0;
        Dropdown_Skill.RefreshShownValue();
    }

    private void SetSkillDropdown(List<string> skills)
    {
        foreach (string skillID in skills)
        {
            Skill skill = GameDataManager.Inst.GetData<Skill>(skillID);

            if (skill == null)
            {
                continue;
            }

            Dropdown_Skill.options.Add(new TMP_Dropdown.OptionData(skill.Name));
            _skillID.Add(skillID);
        }
    }

    private void SetDiceEdit()
    {
        ClearEditSlot();

        _playerEditDice.Clear();
        _opponentEditDice.Clear();

        var data = GameDataManager.Inst.GetAllData<DiceEdit>();

        foreach (DiceEdit dice in data)
        {
            if (dice.ID.Contains(_diceID) == true)
            {
                if (dice.Target == "Player")
                {
                    _playerEditDice.Add(dice.ID);

                    DiceEditSlot slot = _editPool.Get();
                    _activeSlot.Add(slot);

                    slot.SetEditInfo(dice.ID, false);

                    if (dice.EditValue > 0)
                    {
                        slot.transform.SetParent(Parent_PlayerBonusEdit);
                        _playerBonusSlots.Add(slot);
                    }
                    else
                    {
                        slot.transform.SetParent(Parent_PlayerMinusEdit);
                        _playerMinusSlots.Add(slot);
                    }
                }
                else
                {
                    _opponentEditDice.Add(dice.ID);

                    DiceEditSlot slot = _editPool.Get();
                    _activeSlot.Add(slot);

                    slot.SetEditInfo(dice.ID, true);
                    _opponentSlots.Add(slot);

                    if (dice.EditValue > 0)
                    {
                        slot.transform.SetParent(Parent_OpponentBonusEdit);
                    }
                    else
                    {
                        slot.transform.SetParent(Parent_OpponentMinusEdit);
                    }
                }
            }
        }

        SetOpponentSkill();
    }

    private void SetOpponentSkill()
    {
        string opponent = GameDataManager.Inst.GetData<Dice>(_diceID).Opponent;

        if (opponent == "System")
        {
            return;
        }

        _characterVM.SetCurrentCharacter(opponent);

        string maxSkill = "";
        int maxValue = -999;

        foreach (string editID in _opponentEditDice)
        {
            var data = GameDataManager.Inst.GetData<DiceEdit>(editID);

            if (data.EditType == "Skill")
            {
                Enum.TryParse(GameDataManager.Inst.GetData<Skill>(data.EditTarget).Category, out SkillCategory category);

                if (_characterVM.CheckSkill(category, data.EditTarget) == true && maxValue < data.EditValue)
                {
                    maxValue = data.EditValue;
                    maxSkill = data.EditTarget;
                }
            }
        }

        if (maxSkill == "")
        {
            Text_OpponentSkill.text = GameDataManager.Inst.GetData<Skill>(_characterVM.CurrentCharacterModel.SelectedNature).Name;
        }
        else
        {
            Text_OpponentSkill.text = GameDataManager.Inst.GetData<Skill>(maxSkill).Name;
        }

        _characterVM.SetCurrentCharacter("Player_01");
    }

    private void RollNormalDice()
    {
        if (_isRolling)
        {
            return;
        }

        _completeDiceCount = 0;
        _isRolling = true;

        _playerOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();
        _opponentOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();

        _playerOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();
        _opponentOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();

        (int plus, int minus) player = EditDice(true);
        (int plus, int minus) opponent = EditDice(false);

        _playerDice = _playerOriginDice + player.plus + player.minus;
        _opponentDice = _opponentOriginDice + opponent.plus + opponent.minus;

        _playerDiceController.Roll(_playerOriginDice, player.plus, player.minus);
        _opponentDiceController.Roll(_opponentOriginDice, opponent.plus, opponent.minus);
    }

    private void CompleteDiceRoll()
    {
        _completeDiceCount++;

        if (_completeDiceCount < 2)
        {
            return;
        }

        _isRolling = false;

        _diceResult = CheckResult();

        if (_playerDice == _opponentDice)
        {
            _nullCount++;

            if ((_playerDice == 20 && _opponentDice == 20) || (_playerDice == 1 && _opponentDice == 1) || _nullCount >= 2)
            {
                _special = true;
            }

            return;
        }

        Text_OpponentDice.text = $"{_opponentDice}";
        Text_PlayerDice.text = $"{_playerDice}";

        CheckWin();
    }

    private void RollFailDice()
    {
        _playerDice = GameUtil.RollFail();

        ApplyDiceResult(_playerDice);

        Text_PlayerDice.text = $"{_playerDice}";
        _canClose = true;
    }

    private void CheckWin()
    {
        var data = GameDataManager.Inst.GetData<Dice>(_diceID);

        bool isWin;

        if (_playerOriginDice == 20)
        {
            isWin = true;
        }
        else if (_playerOriginDice == 1)
        {
            isWin = false;
        }
        else
        {
            isWin = _opponentDice < _playerDice;
        }

        if (isWin == true)
        {
            TextAdventureManager.Inst.SetCurrentID(data.WinID);
            _canClose = true;
        }
        else
        {
            TextAdventureManager.Inst.SetCurrentID(data.FailID);
        }

        Button_NormalDice.gameObject.SetActive(false);
    }

    private (int plus, int minus) EditDice(bool isPlayer)
    {
        var data = GameDataManager.Inst.GetData<Dice>(_diceID);

        int plus = 0;
        int minus = 0;

        if (data.Opponent == "System" && isPlayer == false)
        {
            return (0, 0);
        }

        List<string> list = isPlayer ? _playerEditDice : _opponentEditDice;

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter(data.Opponent);
        }

        foreach (string editID in list)
        {
            int value = CalculateEditValue(editID, isPlayer);

            if (value > 0)
            {
                plus += value;
            }
            else if (value < 0)
            {
                minus += value;
            }
        }

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter("Player_01");
        }

        return (plus, minus);
    }

    private int CalculateEditValue(string editID, bool isPlayer)
    {
        var data = GameDataManager.Inst.GetData<DiceEdit>(editID);

        switch (data.EditType)
        {
            case "Skill":
                string skill = data.EditTarget;

                if (isPlayer == true)
                {
                    if (Dropdown_Skill.value >= 0 && Dropdown_Skill.value < _skillID.Count)
                    {
                        string selectedSkill = _skillID[Dropdown_Skill.value];

                        if (skill == selectedSkill)
                        {
                            return data.EditValue;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    Enum.TryParse(GameDataManager.Inst.GetData<Skill>(skill).Category, out SkillCategory category);

                    if (_characterVM.CheckSkill(category, skill) == true)
                    {
                        return data.EditValue;
                    }
                    else
                    {
                        return 0;
                    }
                }

                return 0;

            case "Relation":
                string relationName = data.RelationType;
                string relationTarget = data.EditTarget;

                Enum.TryParse(relationName, out RelationType relation);

                if (_characterVM.CurrentCharacterModel.GetRelation(relationTarget, relation) != null)
                {
                    return data.EditValue;
                }
                else
                {
                    return 0;
                }

            case "Ideology":
                int currentIdeology = _characterVM.Ideology;
                int.TryParse(data.EditTarget, out int ideology);

                if (ideology == currentIdeology)
                {
                    return data.EditValue;
                }
                else
                {
                    return 0;
                }

            default:
                return 0;
        }
    }

    private void ApplyDiceResult(int dice)
    {
        string result = GameDataManager.Inst.GetData<Dice>(_diceID).Result;

        switch (result)
        {
            case "Stamina":
                _characterVM.AddStamina(-dice);
                break;

            case "Crazy":
                _characterVM.AddCrazy(dice);
                break;
        }
    }

    private DiceResult CheckResult()
    {
        int middle = _playerDice - _opponentDice;

        if (_playerDice == 20)
        {
            _boost = true;
            return DiceResult.CriticalSuccess;
        }
        else if (_playerDice == 1)
        {
            return DiceResult.CriticalFail;
        }

        int half = _boost ? Mathf.CeilToInt(_opponentDice / 2f) : Mathf.FloorToInt(_opponentDice / 2f);

        if (middle < 20 && middle > _opponentDice)
        {
            _boost = true;
            return DiceResult.ExtremeSuccess;
        }
        else if (middle > half && middle <= _opponentDice)
        {
            return DiceResult.HardSuccess;
        }
        else if (middle > 0 && middle <= half)
        {
            return DiceResult.ReasonSuccess;
        }
        else if (middle == 0)
        {
            return DiceResult.Null;
        }
        else if (middle < 0)
        {
            return DiceResult.Fail;
        }

        return DiceResult.Null;
    }

    private void ClearEditSlot()
    {
        foreach (DiceEditSlot slot in _activeSlot)
        {
            _editPool.Release(slot);
        }

        _activeSlot.Clear();
        _playerBonusSlots.Clear();
        _playerMinusSlots.Clear();
        _opponentSlots.Clear();
    }

    private void OnPlayerPlusHit(int value)
    {
        PlayAppliedSlots(_playerBonusSlots);
    }

    private void OnPlayerMinusHit(int value)
    {
        PlayAppliedSlots(_playerMinusSlots);
    }

    private void OnOpponentHit()
    {
        PlayAppliedSlots(_opponentSlots);
    }

    private void PlayAppliedSlots(List<DiceEditSlot> slots)
    {
        foreach (DiceEditSlot slot in slots)
        {
            slot.PlayHit();
        }
    }
}
