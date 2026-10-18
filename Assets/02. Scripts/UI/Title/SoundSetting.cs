using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer AudioMixer;

    [SerializeField] private Slider Slider_Master;
    [SerializeField] private Slider Slider_BGM;
    [SerializeField] private Slider Slider_SFX;
    [SerializeField] private Slider Slider_Voice;

    private float _defaultVolume = 0.5f;

    private const string Master = "Master";
    private const string BGM = "BGM";
    private const string SFX = "SFX";
    private const string Voice = "Voice";

    private void Start()
    {
        Slider_Master.value = PlayerPrefs.GetFloat(Master, _defaultVolume);
        Slider_BGM.value = PlayerPrefs.GetFloat(BGM, _defaultVolume);
        Slider_SFX.value = PlayerPrefs.GetFloat(SFX, _defaultVolume);
        Slider_Voice.value = PlayerPrefs.GetFloat(Voice, _defaultVolume);

        SetMaster(Slider_Master.value);
        SetBGM(Slider_BGM.value);
        SetSFX(Slider_SFX.value);
        SetVoice(Slider_Voice.value);

        Slider_Master.onValueChanged.AddListener(SetMaster);
        Slider_BGM.onValueChanged.AddListener(SetBGM);
        Slider_SFX.onValueChanged.AddListener(SetSFX);
        Slider_Voice.onValueChanged.AddListener(SetVoice);
    }

    private void SetMaster(float value)
    {
        AudioMixer.SetFloat(Master, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);   // 슬라이더 값을 데시벨 단위로 변환
        PlayerPrefs.SetFloat(Master, value);
    }
    
    private void SetBGM(float value)
    {
        AudioMixer.SetFloat(BGM, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(BGM, value);
    }

    private void SetSFX(float value)
    {
        AudioMixer.SetFloat(SFX, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(SFX, value);
    }

    private void SetVoice(float value)
    {
        AudioMixer.SetFloat(Voice, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(Voice, value);
    }

    public void ResetSound()
    {
        PlayerPrefs.SetFloat(Master, _defaultVolume);
        PlayerPrefs.SetFloat(BGM, _defaultVolume);
        PlayerPrefs.SetFloat(SFX, _defaultVolume);
        PlayerPrefs.SetFloat(Voice, _defaultVolume);

        SetMaster(_defaultVolume);
        SetBGM(_defaultVolume);
        SetSFX(_defaultVolume);
        SetVoice(_defaultVolume);

        Slider_Master.value = _defaultVolume;
        Slider_BGM.value = _defaultVolume;
        Slider_SFX.value = _defaultVolume;
        Slider_Voice.value = _defaultVolume;
    }
}
