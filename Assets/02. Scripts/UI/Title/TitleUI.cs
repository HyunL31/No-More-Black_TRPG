using Cysharp.Threading.Tasks;
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
    [SerializeField] private Button Button_Reset;
    [SerializeField] private Button Button_Sound;
    [SerializeField] private Button Button_Display;
    [SerializeField] private Button Button_Return;

    [Header("패널 스위처")]
    [SerializeField] private PanelSwitcher Switcher_Option;
    [SerializeField] private PanelSwitcher Switcher_Setting;

    [Header("패널")]
    [SerializeField] private PanelSlideController Controller_Title;
    [SerializeField] private PanelSlideController Controller_Setting;
    [SerializeField] private PanelSlideController Controller_Sound;
    [SerializeField] private PanelSlideController Controller_Display;
    [SerializeField] private PanelSlideController Controller_Save;

    [Header("설정")]
    [SerializeField] private SoundSetting SoundSetting;
    [SerializeField] private DisplaySetting DisplaySetting;

    private float _fadeSpeed = 0.3f;
    private float _longDelay = 0.5f;
    private float _shortDelay = 0.2f;
    private CancellationTokenSource _cancel;

    private void Awake()
    {
        Button_NewGame.onClick.AddListener(OnClickNewGame);
        Button_LoadGame.onClick.AddListener(OnClickLoadGame);
        Button_Config.onClick.AddListener(OnClickConfig);
        Button_Quit.onClick.AddListener(OnClickQuit);
        Button_Reset.onClick.AddListener(OnClickReset);
        Button_Sound.onClick.AddListener(OnClickSound);
        Button_Display.onClick.AddListener(OnClickDisplay);
        Button_Return.onClick.AddListener(OnClickReturn);
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

    private void OnClickNewGame()
    {
        // 예비
        string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        PlayerModel player = SaveManager.Inst.RequestLoadData(date);
        SaveManager.Inst.RequestSaveData(player);

        UIManager.Inst.OpenVisualNovelUI();
        UIManager.Inst.OpenDialogueUI();
        UIManager.Inst.CloseTitleUI();
    }

    private void OnClickLoadGame()
    {
        Controller_Save.gameObject.SetActive(true);
    }

    private void OnClickConfig()
    {
        if (Controller_Save.gameObject.activeSelf)
        {
            Controller_Save.SlideOut(Vector2.left);
            Controller_Save.gameObject.SetActive(false);
        }

        SwitchSetting().Forget();
    }

    private async UniTask SwitchSetting()
    {
        Switcher_Option.SwitchTo(Controller_Setting, Vector2.left, Vector2.left).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));

        Controller_Sound.SlideIn(Vector2.right);
    }

    private void OnClickQuit()
    {
        Application.Quit();
    }

    private void OnClickReset()
    {
        SoundSetting.ResetSound();
        DisplaySetting.ResetDisplay();
    }

    private void OnClickSound()
    {
        Switcher_Setting.SwitchTo(Controller_Sound, Vector2.right, Vector2.right).Forget();
    }

    private void OnClickDisplay()
    {
        Switcher_Setting.SwitchTo(Controller_Display, Vector2.right, Vector2.right).Forget();
    }

    private void OnClickReturn()
    {
        PlayerPrefs.Save();

        Switcher_Option.SwitchTo(Controller_Title, Vector2.left, Vector2.left).Forget();
        Controller_Sound.SlideOut(Vector2.right);
        Controller_Display.SlideOut(Vector2.right);
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