using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_Time;

    private int _remainTime;
    private CancellationTokenSource _cancel;

    public Action OnTimeOver;

    private void OnDisable()
    {
        CancelTime();
    }

    public void StartTimer(int limit)
    {
        CancelTime();

        _remainTime = limit;
        Text_Time.text = $"{_remainTime}";

        _cancel = new CancellationTokenSource();

        Countdown(_cancel.Token).Forget();
    }

    private async UniTask Countdown(CancellationToken token)
    {
        while (_remainTime > 0)
        {
            await UniTask.Delay(1000, cancellationToken: token);

            _remainTime--;
            UpdateTimerText();
        }

        OnTimeOver?.Invoke();

        _cancel?.Dispose();
        _cancel = null;
    }

    private void UpdateTimerText()
    {
        Text_Time.text = _remainTime.ToString();
    }

    private void CancelTime()
    {
        if (_cancel != null)
        {
            _cancel.Cancel();
            _cancel.Dispose();
            _cancel = null;
        }
    }
}
