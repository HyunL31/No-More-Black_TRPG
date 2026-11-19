using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceResultUI : ViewBase
{
    [Header("패널")]
    [SerializeField] private PanelSlideController Panel_PlayerSuccess;
    [SerializeField] private PanelSlideController Panel_OpponentSuccess;
    [SerializeField] private PanelSlideController Panel_PlayerFail;
    [SerializeField] private PanelSlideController Panel_OpponentFail;
    [SerializeField] private DiceFailUI Panel_DiceFail;
    [SerializeField] private GameObject Panel_ResultPanel;

    [SerializeField] private Button Button_Close;

    [Header("결과 패널")]
    [SerializeField] private PanelSlideController Slide_PlayerSuccess;
    [SerializeField] private PanelSlideController Slide_OpponentSuccess;
    [SerializeField] private PanelSlideController Slide_PlayerFail;
    [SerializeField] private PanelSlideController Slide_OpponentFail;

    [Header("결과 텍스트")]
    [SerializeField] private TextMeshProUGUI Text_PlayerSuccess;
    [SerializeField] private TextMeshProUGUI Text_OpponentSuccess;
    [SerializeField] private TextMeshProUGUI Text_PlayerFail;
    [SerializeField] private TextMeshProUGUI Text_OpponentFail;

    private DiceResult _playerResult = DiceResult.None;
    private DiceResult _opponentResult = DiceResult.None;

    private bool _isWin = false;
    private bool _canClose = false;

    private Dictionary<string, CharacterState> _diceCharacter = new Dictionary<string, CharacterState>();

    private string _player = "Player_01";
    private string _opponent = "";
    private string _diceID;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnDisable()
    {
        Panel_PlayerSuccess.gameObject.SetActive(false);
        Panel_PlayerFail.gameObject.SetActive(false);
        Panel_OpponentSuccess.gameObject.SetActive(false);
        Panel_OpponentFail.gameObject.SetActive(false);

        Slide_PlayerSuccess.gameObject.SetActive(false);
        Slide_PlayerFail.gameObject.SetActive(false);
        Slide_OpponentSuccess.gameObject.SetActive(false);
        Slide_OpponentFail.gameObject.SetActive(false);

        Panel_DiceFail.gameObject.SetActive(false);
        Panel_ResultPanel.SetActive(true);

        ClearCharacter();

        _canClose = false;
    }

    private void OnClickClose()
    {
        if (_canClose == false)
        {
            return;
        }
        else
        {
            if (_isWin == true)
            {
                UIManager.Inst.OpenDialogueUI();
                GameObjectManager.Inst.CloseDice();
                UIManager.Inst.CloseDiceUI();
                UIManager.Inst.CloseDiceResultUI();
            }
            else
            {
                UIManager.Inst.OpenDialogueUI();
                GameObjectManager.Inst.CloseDice();
                UIManager.Inst.CloseDiceUI();

                Panel_ResultPanel.SetActive(false);
                
                Panel_DiceFail.gameObject.SetActive(true);
                Panel_DiceFail.SetDiceID(_diceID);
                Panel_DiceFail.ApplyDiceResult();
            }
        }
    }

    public void SetDiceResult(string diceID, bool isWin, DiceResult player, DiceResult opponent)
    {
        _diceID = diceID;

        _opponent = GameDataManager.Inst.GetData<Dice>(_diceID).Opponent;
        _isWin = isWin;

        _playerResult = player;
        _opponentResult = opponent;

        ApplyCharacter().Forget();

        ShowResultSequence().Forget();
    }

    private async UniTask<CharacterState> GetCharacter(string id)
    {
        if (_diceCharacter.ContainsKey(id))
        {
            _diceCharacter[id].gameObject.SetActive(true);
            return _diceCharacter[id];
        }

        GameObject spine = await ResourceManager.Inst.InstantiateAsync($"Spine/{id}");

        CharacterState state = spine.GetComponent<CharacterState>();
        _diceCharacter.Add(id, state);

        return state;
    }

    private void SetCharacter(CharacterState state, Transform parent, string skin)
    {
        state.transform.SetParent(parent, false);
        state.ApplyEmotion("basic", skin);
        state.FitFace(parent);
    }

    private async UniTask ApplyCharacter()
    {
        CharacterState player = await GetCharacter(_player);
        CharacterState opponent = await GetCharacter(_opponent);

        if (_isWin == true)
        {
            SetCharacter(player, Panel_PlayerSuccess.transform, "default");
            SetCharacter(opponent, Panel_OpponentFail.transform, "normal");
        }
        else
        {
            SetCharacter(player, Panel_PlayerFail.transform, "default");
            SetCharacter(opponent, Panel_OpponentSuccess.transform, "smile");
        }
    }

    private async UniTask ShowResultSequence()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        ShowPanel();

        await UniTask.Delay(TimeSpan.FromSeconds(0.7f));

        ShowText();
    }

    private void ShowPanel()
    {
        Panel_PlayerSuccess.gameObject.SetActive(_isWin);
        Panel_OpponentFail.gameObject.SetActive(_isWin);

        Panel_PlayerFail.gameObject.SetActive(!_isWin);
        Panel_OpponentSuccess.gameObject.SetActive(!_isWin);
    }

    private void ShowText()
    {
        Slide_PlayerSuccess.gameObject.SetActive(_isWin);
        Slide_OpponentFail.gameObject.SetActive(_isWin);

        Slide_PlayerFail.gameObject.SetActive(!_isWin);
        Slide_OpponentSuccess.gameObject.SetActive(!_isWin);

        if (_isWin == true)
        {
            Text_PlayerSuccess.text = ReturnDiceResult(_playerResult);
            Text_OpponentFail.text = ReturnDiceResult(_opponentResult);
        }
        else
        {
            Text_PlayerFail.text = ReturnDiceResult(_playerResult);
            Text_OpponentSuccess.text = ReturnDiceResult(_opponentResult);
        }

        _canClose = true;
    }
    
    private void ClearCharacter()
    {
        foreach (CharacterState character in _diceCharacter.Values)
        {
            character.gameObject.SetActive(false);
        }
    }

    private string ReturnDiceResult(DiceResult result)
    {
        switch (result)
        {
            case DiceResult.CriticalSuccess:
                return "대성공";

            case DiceResult.ExtremeSuccess:
                return "극단적 성공";

            case DiceResult.HardSuccess:
                return "어려운 성공";

            case DiceResult.ReasonSuccess:
                return "보통 성공";

            case DiceResult.Null:
                return "무효";

            case DiceResult.Fail:
                return "실패";

            case DiceResult.CriticalFail:
                return "대실패";

            default:
                return "";
        }
    }
}
