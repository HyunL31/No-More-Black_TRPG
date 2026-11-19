using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceFailUI : ViewBase
{
    [SerializeField] private TextMeshProUGUI Text_Prefix;
    [SerializeField] private TextMeshProUGUI Text_Suffix;
    [SerializeField] private TextMeshProUGUI Text_Value1;
    [SerializeField] private TextMeshProUGUI Text_Value2;

    [SerializeField] private RectTransform Rect_Value1;
    [SerializeField] private RectTransform Rect_Value2;

    [SerializeField] private Button Button_Confirm;

    private Vector2 _originPos;
    private int _cycles = 2;
    private float _fast = 0.05f;
    private float _slow = 0.22f;
    private float _height = 357f;

    private int _value;
    private int _result;

    private CancellationTokenSource _cancel;

    private CharacterViewModel _characterVM;
    private string _diceID;

    private Action<int> _onConfirm;

    private void Awake()
    {
        Button_Confirm.onClick.AddListener(OnClickConfirm);

        _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        _originPos = Rect_Value1.anchoredPosition;
    }

    private void OnClickConfirm()
    {
        _onConfirm?.Invoke(_result);
        UIManager.Inst.CloseDiceResultUI();
    }

    public void SetDiceID(string diceID)
    {
        _diceID = diceID;
    }

    public void ApplyDiceResult()
    {
        string result = GameDataManager.Inst.GetData<Dice>(_diceID).Result;

        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        }

        _characterVM.SetCurrentCharacter("Player_01");

        switch (result)
        {
            case "Stamina":
                SetRollInfoText("체력이 ", "만큼 감소합니다.", 1, 3, (value) => _characterVM.AddStamina(-value)).Forget();
                break;

            case "Crazy":
                SetRollInfoText("열광이 ", "만큼 상승합니다.", 1, 3, (value) => _characterVM.AddCrazy(value)).Forget();
                break;
        }
    }

    private void SetValue (int value)
    {
        _value = value;
        Text_Value1.text = value.ToString();

        Rect_Value1.anchoredPosition = _originPos;
        Rect_Value2.anchoredPosition = _originPos - new Vector2(0f, -_height);
    }

    private async UniTask Roll(int min, int max, int target, CancellationToken token)
    {
        int len = max - min + 1;
        int total = len * _cycles + (target - min);
        SetValue(min);

        for (int i = 1; i <= total; i++)
        {
            int next = min + ((_value - min + 1) % len);
            Text_Value2.text = next.ToString();
            Rect_Value2.anchoredPosition = _originPos - new Vector2(0f, -_height);

            float p = (float)i / total;
            float dur = Mathf.Lerp(_fast, _slow, p * p);
            Ease ease = (i == total) ? Ease.OutBack : Ease.Linear;

            Sequence sequence = DOTween.Sequence().SetUpdate(true).Join(Rect_Value1.DOAnchorPosY(_originPos.y - _height, dur).SetEase(ease)).Join(Rect_Value2.DOAnchorPosY(_originPos.y, dur).SetEase(ease));

            await UniTask.WaitUntil(() => !sequence.IsActive(), cancellationToken: token);

            (Text_Value1, Text_Value2) = (Text_Value2, Text_Value1);
            (Rect_Value1, Rect_Value2) = (Rect_Value2, Rect_Value1);
            _value = next;
        }

    }

    public async UniTask SetRollInfoText(string prefix, string suffix, int min, int max, Action<int> onConfirm)
    {
        CancelRoll();

        Button_Confirm.interactable = false;

        Text_Prefix.text = prefix;
        Text_Suffix.text = suffix;

        _result = Random.Range(min, max + 1);
        _onConfirm = onConfirm;

        _cancel = new CancellationTokenSource();

        await Roll(min, max, _result, _cancel.Token);

        Button_Confirm.interactable = true;
    }

    private void CancelRoll()
    {
        if (_cancel != null)
        {
            _cancel.Cancel();
            _cancel.Dispose();
            _cancel = null;
        }

        Rect_Value1.DOKill();
        Rect_Value2.DOKill();
    }
}
