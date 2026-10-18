using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown Dropdown_Resolution;
    [SerializeField] private Toggle Toggle_FullScreen;
    [SerializeField] private Slider Slider_Brightness;
    [SerializeField] private TMP_Dropdown Dropdown_TextSpeed;
    [SerializeField] private Toggle Toggle_Vibration;

    private int _defaultResolution = 0;
    private bool _defaultFullScreen = true;
    private float _defaultBrightness = 0.0f;
    private bool _defaultVibration = true;

    private const string Resolution = "Resolution";
    private const string FullScreen = "FullScreen";
    private const string Brightness = "Brightness";
    private const string Vibration = "Vibration";
    private const string TextSpeed = "TextCPS";
    private const string TextIdx = "TextIndex";

    private Resolution[] _resolutions =
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 }
    };

    private float[] _textSpeed = { 20f, 40f, 80f, 9999f };

    private void Start()
    {
        GetResolutionOptions();
        GetTextSpeedOptions();

        Dropdown_Resolution.value = PlayerPrefs.GetInt(Resolution, 0);
        Dropdown_Resolution.RefreshShownValue();
        Dropdown_Resolution.onValueChanged.AddListener(SetResolution);

        Toggle_FullScreen.isOn = PlayerPrefs.GetInt(FullScreen, 1) == 1;
        Toggle_FullScreen.onValueChanged.AddListener(SetFullScreen);

        Slider_Brightness.value = PlayerPrefs.GetFloat(Brightness, 0);
        GameManager.Inst.OnChangedBrightness?.Invoke(Slider_Brightness.value);
        Slider_Brightness.onValueChanged.AddListener(SetBrightness);

        Dropdown_TextSpeed.value = PlayerPrefs.GetInt(TextIdx, 1);
        Dropdown_TextSpeed.RefreshShownValue();
        Dropdown_TextSpeed.onValueChanged.AddListener(SetTextSpeed);

        Toggle_Vibration.isOn = PlayerPrefs.GetInt(Vibration, 1) == 1;
        Toggle_Vibration.onValueChanged.AddListener(SetVibration);
    }

    private void GetResolutionOptions()
    {
        Dropdown_Resolution.ClearOptions();

        var options = new List<string>(_resolutions.Length);

        foreach (var resolution in _resolutions)
        {
            options.Add($"{resolution.width} X {resolution.height}");
        }

        Dropdown_Resolution.AddOptions(options);
    }

    private void GetTextSpeedOptions()
    {
        Dropdown_TextSpeed.ClearOptions();
        Dropdown_TextSpeed.AddOptions(new List<string> { "느림", "보통", "빠름", "즉시" });
    }

    private void SetResolution(int idx)
    {
        var resolution = _resolutions[idx];
        Screen.SetResolution(resolution.width, resolution.height, Toggle_FullScreen.isOn);

        PlayerPrefs.SetInt(Resolution, idx);
    }

    private void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;

        int full = isFull ? 1 : 0;
        PlayerPrefs.SetInt(FullScreen, full);
    }

    private void SetBrightness(float value)
    {
        GameManager.Inst.OnChangedBrightness?.Invoke(Slider_Brightness.value);

        PlayerPrefs.SetFloat(Brightness, value);
    }

    private void SetTextSpeed(int idx)
    {
        float cps = _textSpeed[idx];

        PlayerPrefs.SetInt(TextIdx, idx);
        PlayerPrefs.SetFloat(TextSpeed, cps);
    }

    public void ResetDisplay()
    {
        Dropdown_Resolution.value = _defaultResolution;
        Dropdown_Resolution.RefreshShownValue();

        Toggle_FullScreen.isOn = _defaultFullScreen;

        Slider_Brightness.value = _defaultBrightness;

        Toggle_Vibration.isOn = _defaultVibration;

        Dropdown_Resolution.value = _defaultResolution;
        Dropdown_TextSpeed.RefreshShownValue();

        SetResolution(Dropdown_Resolution.value);
        SetBrightness(Slider_Brightness.value);
        SetTextSpeed(Dropdown_TextSpeed.value);
        SetVibration(Toggle_Vibration.isOn);
    }

    private void SetVibration(bool isOn)
    {
        PlayerPrefs.SetInt(Vibration, isOn ? 1 : 0);

        if (isOn)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
 Handheld.Vibrate();
#endif
        }
    }
}
