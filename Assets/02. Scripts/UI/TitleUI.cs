using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : ViewBase
{
    [Header("시작 패널")]
    [SerializeField] private CanvasGroup Panel_Start;
    [SerializeField] private CanvasGroup Panel_Trigger;
    [SerializeField] private CanvasGroup Panel_Title;

    [Header("버튼")]
    [SerializeField] private Button Button_NewGame;
    [SerializeField] private Button Button_LoadGame;
    [SerializeField] private Button Button_Config;
    [SerializeField] private Button Button_Quit;

    private float _fadeSpeed = 0.3f;
    private float _longDelay = 0.5f;
    private float _shortDelay = 0.2f;
    private CancellationTokenSource _cancel;

    private void Awake()
    {
        
    }

    private void Start()
    {
        Panel_Start.alpha = 1f;
        Panel_Trigger.alpha = 0f;
        Panel_Title.alpha = 0f;

        CancelFade();
        _cancel = new CancellationTokenSource();

        FadeRoutine(_cancel.Token).Forget();
    }

    private void OnDestroy()
    {
        CancelFade();
    }

    private async UniTask FadeRoutine(CancellationToken cancellationToken)
    {
        await FadeOut(Panel_Start, cancellationToken);
        await UniTask.Delay(TimeSpan.FromSeconds(_longDelay), cancellationToken: cancellationToken);

        await FadeIn(Panel_Trigger, cancellationToken);
        await UniTask.Delay(TimeSpan.FromSeconds(_longDelay), cancellationToken: cancellationToken);

        Panel_Start.gameObject.SetActive(false);

        await FadeOut(Panel_Trigger, cancellationToken);
        await UniTask.Delay(TimeSpan.FromSeconds(_shortDelay), cancellationToken: cancellationToken);

        await FadeIn(Panel_Title, cancellationToken);
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);

        Panel_Trigger.gameObject.SetActive(false);
    }

    private async UniTask FadeIn(CanvasGroup panel, CancellationToken cancellationToken)
    {
        float elapsed = 0f;

        while (_fadeSpeed > elapsed)
        {
            elapsed += Time.deltaTime;

            panel.alpha = Mathf.Lerp(0, 1, elapsed / _fadeSpeed);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        if (panel != null)
        {
            panel.alpha = 1f;
        }
    }

    private async UniTask FadeOut(CanvasGroup panel, CancellationToken cancellationToken)
    {
        float elapsed = 0f;

        while (_fadeSpeed > elapsed)
        {
            elapsed += Time.deltaTime;

            panel.alpha = Mathf.Lerp(1, 0, elapsed / _fadeSpeed);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        if (panel != null)
        {
            panel.alpha = 0;
        }
    }

    private void CancelFade()
    {
        if (_cancel != null)
        {
            _cancel.Cancel();
            _cancel.Dispose();
            _cancel = null;
        }
    }
}