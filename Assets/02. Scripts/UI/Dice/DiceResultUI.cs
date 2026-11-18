using Cysharp.Threading.Tasks;
using System;
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
            UIManager.Inst.OpenDialogueUI();
            GameObjectManager.Inst.CloseDice();
            UIManager.Inst.CloseDiceUI();
            UIManager.Inst.CloseDiceResultUI();
        }
    }

    public void SetDiceResult(bool isWin, DiceResult player, DiceResult opponent)
    {
        _isWin = isWin;

        _playerResult = player;
        _opponentResult = opponent;

        ShowResultSequence().Forget();
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
