using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_NormalDice;
    [SerializeField] private Button Button_FailDice;
    [SerializeField] private Button Button_Result;

    [SerializeField] private TextMeshProUGUI Text_OpponentTitle;

    [SerializeField] private TextMeshProUGUI Text_OpponentDice;
    [SerializeField] private TextMeshProUGUI Text_OpponentResult;

    [SerializeField] private TextMeshProUGUI Text_PlayerDice;
    [SerializeField] private TextMeshProUGUI Text_PlayerResult;

    [SerializeField] private TMP_Dropdown Dropdown_Skill;

    [Header("테스트용")]
    [SerializeField] private GameObject Panel_TestResult;
    [SerializeField] private Button Button_TestClose;
    [SerializeField] private TextMeshProUGUI Text_Opponent;
    [SerializeField] private TextMeshProUGUI Text_PlayerOriginDice;
    [SerializeField] private TextMeshProUGUI Text_OpponentOriginDice;
    [SerializeField] private TextMeshProUGUI Text_PlayerEdit;
    [SerializeField] private TextMeshProUGUI Text_OpponentEdit;
    [SerializeField] private TextMeshProUGUI Text_PlayerEditResult;
    [SerializeField] private TextMeshProUGUI Text_OpponentEditResult;
    [SerializeField] private TextMeshProUGUI Text_PlayerFinal;
    [SerializeField] private TextMeshProUGUI Text_FailResult;
    [SerializeField] private TextMeshProUGUI Text_FailApply;
    [SerializeField] private TextMeshProUGUI Text_Boost;

    private CharacterViewModel _characterVM;

    private DiceController _playerDiceController;
    private DiceController _opponentDiceController;
    private bool _isRolling = false;
    private bool _canClose = false;

    private string _diceID;
    private bool _boost = false;
    private bool _special = false;
    private int _nullCount = 0;

    private int _playerOriginDice;
    private int _opponentOriginDice;

    private int _playerDice;
    private int _opponentDice;

    private DiceResult _diceResult;
    private List<string> _skillID = new List<string>();


    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
        Button_NormalDice.onClick.AddListener(OnClickDice);
        Button_FailDice.onClick.AddListener(OnClickFailDice);
        Button_Result.onClick.AddListener(OnClickResult);
        Button_TestClose.onClick.AddListener(OnClickTestClose);

        _characterVM = ServiceManager.Inst.GetCharacterViewModel();
    }

    private void OnEnable()
    {
        _nullCount = 0;
        _special = false;
        _canClose = false;

        Button_NormalDice.gameObject.SetActive(true);
        Button_FailDice.gameObject.SetActive(false);

        _isRolling = false;

        Text_OpponentDice.text = "";
        Text_OpponentResult.text = "";
        Text_PlayerDice.text = "";
        Text_PlayerResult.text = "";

        Panel_TestResult.gameObject.SetActive(false);

        Text_Opponent.text = "";
        Text_PlayerOriginDice.text = "";
        Text_OpponentOriginDice.text = "";
        Text_PlayerEdit.text = "";
        Text_OpponentEdit.text = "";
        Text_PlayerEditResult.text = "";
        Text_OpponentEditResult.text = "";
        Text_PlayerFinal.text = "";
        Text_FailResult.text = "";
        Text_FailApply.text = "";

        InitDiceObject();
    }

    private void OnDisable()
    {
        _playerDiceController.OnRollComplete -= CompleteDiceRoll;
        _opponentDiceController.OnRollComplete -= CompleteDiceRoll;
    }

    private void OnClickDice()
    {
        RollNormalDice();
        Dropdown_Skill.interactable = false;
    }

    private void OnClickFailDice()
    {
        RollFailDice();
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

    private void OnClickResult()
    {
        GameObjectManager.Inst.CloseDice();
        Panel_TestResult.gameObject.SetActive(true);
        Text_Boost.text = $"고양 상태 여부: {_boost}";
    }

    private void OnClickTestClose()
    {
        Panel_TestResult.gameObject.SetActive(false);
        GameObjectManager.Inst.InitPlayerDice();
        GameObjectManager.Inst.InitOpponentDice();
    }

    public void SetDiceID(string diceID)
    {
        _diceID = diceID;
        InitDropdown();
    }

    public void SetOpponent()
    {
        string opponent = GameDataManager.Inst.GetData<Dice>(_diceID).Opponent;

        if (opponent == "System")
        {
            Text_OpponentTitle.text = $"시스템 다이스";
            Text_Opponent.text = "상대 : 시스템";

            return;
        }

        Text_OpponentTitle.text = $"{GameDataManager.Inst.GetData<Character>(opponent).Name} 다이스";
        Text_Opponent.text = $"상대 : {GameDataManager.Inst.GetData<Character>(opponent).Name}";
    }

    private void InitDiceObject()
    {
        _playerDiceController = GameObjectManager.Inst.InitPlayerDice();
        _opponentDiceController = GameObjectManager.Inst.InitOpponentDice();

        _playerDiceController.OnRollComplete += CompleteDiceRoll;
        _opponentDiceController.OnRollComplete += CompleteDiceRoll;
    }

    private void InitDropdown()
    {
        _characterVM.SetCurrentCharacter("Player_01");
        _skillID.Clear();

        if (GameDataManager.Inst.GetData<Dice>(_diceID).Type != "Skill")
        {
            Dropdown_Skill.gameObject.SetActive(false);
            return;
        }

        Dropdown_Skill.interactable = true;
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

    private void RollNormalDice()
    {
        if (_isRolling)
        {
            return;
        }

        _isRolling = true;

        _playerOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();
        _opponentOriginDice = _special ? GameUtil.RollSpecial() : GameUtil.RollNormal();

        _playerDice = EditDice(_playerOriginDice, true);
        _opponentDice = EditDice(_opponentOriginDice, false);

        _playerDiceController.Roll(_playerOriginDice, _playerDice);
        _opponentDiceController.Roll(_opponentOriginDice, _opponentDice);
    }

    private void CompleteDiceRoll()
    {
        _isRolling = false;

        Text_PlayerOriginDice.text = $"플레이어 원래 다이스: {_playerOriginDice}";
        Text_OpponentOriginDice.text = $"상대방 원래 다이스: {_opponentOriginDice}";

        Text_PlayerEditResult.text = $"플레이어 보정 적용: {_playerDice}";
        Text_OpponentEditResult.text = $"상대방 보정 적용: {_opponentDice}";

        Text_OpponentDice.text = $"{_opponentDice}";
        Text_PlayerDice.text = $"{_playerDice}";

        _diceResult = CheckResult();
        Text_PlayerFinal.text = _diceResult.ToString();

        if (_playerDice == _opponentDice)
        {
            Text_OpponentResult.text = "무효";
            Text_PlayerResult.text = "무효";

            _nullCount++;

            if ((_playerDice == 20 && _opponentDice == 20) || (_playerDice == 1 && _opponentDice == 1) || _nullCount >= 2)
            {
                _special = true;
            }

            return;
        }

        CheckWin();
    }

    private void RollFailDice()
    {
        Text_OpponentDice.text = "";
        Text_OpponentResult.text = "";
        Text_PlayerResult.text = "";

        _playerDice = GameUtil.RollFail();
        Text_FailResult.text = $"실패 결과: {_playerDice}";

        ApplyDiceResult(_playerDice);

        Text_PlayerDice.text = $"{_playerDice}";

        Button_FailDice.gameObject.SetActive(false);
        _canClose = true;
    }

    private void CheckWin()
    {
        var data = GameDataManager.Inst.GetData<Dice>(_diceID);

        Text_OpponentResult.text = _opponentDice > _playerDice ? "승" : "패";
        Text_PlayerResult.text = _opponentDice < _playerDice ? "승" : "패";

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
        Button_FailDice.gameObject.SetActive(isWin == false);
    }

    private int EditDice(int dice, bool isPlayer)
    {
        var data = GameDataManager.Inst.GetData<Dice>(_diceID);

        if (data.Opponent == "System" && isPlayer == false)
        {
            Text_OpponentEdit.text = "시스템: 적용 불가";
            return dice;
        }

        bool canEdit = false;
        bool isBonus = false;

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter(data.Opponent);
        }

        switch (data.Type)
        {
            case "Skill":
                string[] bonusSkill = data.TrueTarget;
                string[] minusSkill = data.FalseTarget;

                if (isPlayer == true)
                {
                    if (Dropdown_Skill.value >= 0 && Dropdown_Skill.value < _skillID.Count)
                    {
                        string selectedSkill = _skillID[Dropdown_Skill.value];

                        if (bonusSkill.Contains(selectedSkill))
                        {
                            canEdit = true;
                            isBonus = true;
                        }
                        else if (minusSkill.Contains(selectedSkill))
                        {
                            canEdit = true;
                            isBonus = false;
                        }
                    }
                }
                else
                {
                    foreach (string skill in bonusSkill)
                    {
                        Enum.TryParse(GameDataManager.Inst.GetData<Skill>(skill).Category, out SkillCategory category);

                        if (_characterVM.CheckSkill(category, skill) == true)
                        {
                            canEdit = true;
                            isBonus = true;

                            break;
                        }
                    }

                    foreach (string skill in minusSkill)
                    {
                        Enum.TryParse(GameDataManager.Inst.GetData<Skill>(skill).Category, out SkillCategory category);

                        if (_characterVM.CheckSkill(category, skill) == true)
                        {
                            canEdit = true;
                            isBonus = false;

                            break;
                        }
                    }
                }

                if (isPlayer)
                {
                    Text_PlayerEdit.text = $"보정 검사: Skill";
                }
                else
                {
                    Text_OpponentEdit.text = $"보정 검사: Skill";
                }

                break;

            case "Relation":
                string[] relations = data.RelationType;

                string[] bonusCharacter = data.TrueTarget;
                string[] minusCharacter = data.FalseTarget;

                foreach (string relationName in relations)
                {
                    Enum.TryParse(relationName, out RelationType relation);

                    foreach (string target in bonusCharacter)
                    {
                        if (_characterVM.CurrentCharacterModel.GetRelation(target, relation) != null)
                        {
                            canEdit = true;
                            isBonus = true;
                            break;
                        }
                    }

                    foreach (string target in minusCharacter)
                    {
                        if (_characterVM.CurrentCharacterModel.GetRelation(target, relation) != null)
                        {
                            canEdit = true;
                            isBonus = false;
                            break;
                        }
                    }

                    if (canEdit == true)
                    {
                        break;
                    }
                }

                if (isPlayer)
                {
                    Text_PlayerEdit.text = $"보정 검사: Relation";
                }
                else
                {
                    Text_OpponentEdit.text = $"보정 검사: Relation";
                }

                break;

            case "Ideology":
                string[] bonusIdeology = data.TrueTarget;
                string[] minusIdeology = data.FalseTarget;

                string currentIdeology = _characterVM.Ideology.ToString();

                if (bonusIdeology.Contains(currentIdeology))
                {
                    canEdit = true;
                    isBonus = true;
                }
                else if (minusIdeology.Contains(currentIdeology))
                {
                    canEdit = true;
                    isBonus = false;
                }

                if (isPlayer)
                {
                    Text_PlayerEdit.text = $"보정 검사: Ideology";
                }
                else
                {
                    Text_OpponentEdit.text = $"보정 검사: Ideology";
                }

                break;
        }

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter("Player_01");
        }

        if (canEdit == true)
        {
            if (isBonus == true)
            {
                return dice + data.EditValue;
            }
            else
            {
                return dice - data.EditValue;
            }
        }
        else
        {
            return dice;
        }
    }

    private void ApplyDiceResult(int dice)
    {
        string result = GameDataManager.Inst.GetData<Dice>(_diceID).Result;

        switch (result)
        {
            case "Stamina":
                _characterVM.AddStamina(-dice);
                Text_FailApply.text = "실패 적용 수치: Stamina";
                break;

            case "Crazy":
                _characterVM.AddCrazy(dice);
                Text_FailApply.text = "실패 적용 수치: Crazy";
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

        int fifth = _boost ? Mathf.CeilToInt(_opponentDice / 5f) : Mathf.FloorToInt(_opponentDice / 5f);
        int half = _boost ? Mathf.CeilToInt(_opponentDice / 2f) : Mathf.FloorToInt(_opponentDice / 2f);

        if (middle < 20 && middle > half)
        {
            _boost = true;
            return DiceResult.ExtremeSuccess;
        }
        else if (middle > fifth && middle <= half)
        {
            return DiceResult.HardSuccess;
        }
        else if (middle > 0 && middle <= fifth)
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
}
