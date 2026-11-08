using System;
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

    private CharacterViewModel _characterVM;

    private string _diceID;
    private bool _boost = false;
    private bool _special = false;
    private int _nullCount = 0;

    private int _playerDice;
    private int _opponentDice;

    private DiceResult _diceResult;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
        Button_NormalDice.onClick.AddListener(OnClickDice);
        Button_FailDice.onClick.AddListener(OnClickFailDice);

        _characterVM = ServiceManager.Inst.GetCharacterViewModel();
    }

    private void OnEnable()
    {
        _nullCount = 0;
        _special = false;

        Button_NormalDice.gameObject.SetActive(true);
        Button_FailDice.gameObject.SetActive(false);

        Text_OpponentDice.text = "";
        Text_OpponentResult.text = "";
        Text_PlayerDice.text = "";
        Text_PlayerResult.text = "";
    }

    private void OnClickDice()
    {
        RollNormalDice();
    }

    private void OnClickFailDice()
    {
        RollFailDice();
    }

    private void OnClickClose()
    {
        UIManager.Inst.OpenDialogueUI();
        UIManager.Inst.CloseDiceUI();
    }

    public void SetDiceID(string diceID)
    {
        _diceID = diceID;
    }

    public void SetOpponent()
    {
        string opponent = GameDataManager.Inst.GetData<Dice>(_diceID).Opponent;

        if (opponent == "System")
        {
            Text_OpponentTitle.text = $"시스템 다이스";

            return;
        }

        Text_OpponentTitle.text = $"{GameDataManager.Inst.GetData<Character>(opponent).Name} 다이스";
    }

    private void RollNormalDice()
    {
        if (_special == true)
        {
            _playerDice = EditDice(GameUtil.RollSpecial(), true);
            _opponentDice = EditDice(GameUtil.RollSpecial(), false);
        }
        else
        {
            _playerDice = EditDice(GameUtil.RollNormal(), true);
            _opponentDice = EditDice(GameUtil.RollNormal(), false);
        }

        Text_OpponentDice.text = $"{_opponentDice}";
        Text_PlayerDice.text = $"{_playerDice}";

        _diceResult = CheckResult();

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

        Text_OpponentResult.text = _opponentDice > _playerDice ? "승" : "패";
        Text_PlayerResult.text = _opponentDice < _playerDice ? "승" : "패";

        var data = GameDataManager.Inst.GetData<Dice>(_diceID);
        bool isWin = _opponentDice < _playerDice;

        if (isWin == true)
        {
            TextAdventureManager.Inst.SetCurrentID(data.WinID);
        }
        else
        {
            TextAdventureManager.Inst.SetCurrentID(data.FailID);
        }

        Button_NormalDice.gameObject.SetActive(false);
        Button_FailDice.gameObject.SetActive(isWin == false);
    }

    private void RollFailDice()
    {
        Text_OpponentDice.text = "";
        Text_OpponentResult.text = "";
        Text_PlayerResult.text = "";

        _playerDice = GameUtil.RollFail();

        ApplyDiceResult(_playerDice);

        Text_PlayerDice.text = $"{_playerDice}";

        Button_FailDice.gameObject.SetActive(false);
    }

    private int EditDice(int dice, bool isPlayer)
    {
        Debug.Log(dice);

        var data = GameDataManager.Inst.GetData<Dice>(_diceID);

        if (data.Opponent == "System")
        {
            return dice;
        }

        int editProperty = 3;
        bool canEdit = false;

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter(data.Opponent);
        }

        switch (data.Type)
        {
            case "Skill":
                string skillID = data.TargetID;
                Enum.TryParse(GameDataManager.Inst.GetData<Skill>(skillID).Category, out SkillCategory category);

                if (_characterVM.CheckSkill(category, skillID) == true)
                {
                    canEdit = true;
                }
                break;

            case "Relation":
                string targetID = data.TargetID;
                Enum.TryParse(targetID, out RelationType relation);

                string character = data.CharacterID;

                if (_characterVM.CurrentCharacterModel.GetRelation(character, relation) != null)
                {
                    canEdit = true;
                }
                break;

            case "Ideology":
                int ideology = int.Parse(data.TargetID);

                if (_characterVM.Ideology == ideology)
                {
                    canEdit = true;
                }
                break;
        }

        if (isPlayer == false)
        {
            _characterVM.SetCurrentCharacter("Player_01");
        }

        if (canEdit == true)
        {
            return dice + editProperty;
        }
        else
        {
            return dice - editProperty;
        }
    }

    private void ApplyDiceResult(int dice)
    {
        string result = GameDataManager.Inst.GetData<Dice>(_diceID).Result;

        switch (result)
        {
            case "Stamina":
                _characterVM.AddStamina(dice);
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
