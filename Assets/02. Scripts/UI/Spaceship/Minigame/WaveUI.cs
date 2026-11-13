using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : ViewBase
{
    [SerializeField] private Slider Slider_Wave;
    [SerializeField] private Button Button_Confirm;

    [SerializeField] private Timer Timer;
    [SerializeField] private int Timer_Limit;
    
    private WaveRenderer _waveRenderer;

    private float _targetFreq;
    private float _playerFreq;

    private float _threshold = 0.5f;

    private void Awake()
    {
        Slider_Wave.minValue = 0f;
        Slider_Wave.maxValue = 65f;

        Slider_Wave.onValueChanged.AddListener(OnChangeSlider);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnEnable()
    {
        UIManager.Inst.CloseBackground();
        InitWave().Forget();

        Timer.OnTimeOver += OnClickConfirm;
        Timer.StartTimer(Timer_Limit);
    }

    private void OnDisable()
    {
        UIManager.Inst.OpenBackground();

        if (_waveRenderer != null)
        {
            _waveRenderer.gameObject.SetActive(false);
        }

        Timer.OnTimeOver -= OnClickConfirm;
    }
    
    private void OnClickConfirm()
    {
        if (CheckAnswer() == true)
        {
            UIManager.Inst.OpenWinPanel();
        }
        else
        {
            UIManager.Inst.OpenFailPanel();
        }
    }

    private void OnChangeSlider(float value)
    {
        _playerFreq = value;
        _waveRenderer.SetPlayerWave(_playerFreq);
    }

    private async UniTask InitWave()
    {
        if (_waveRenderer == null)
        {
            GameObject prefab = await ResourceManager.Inst.InstantiateAsync("WaveRenderer");
            _waveRenderer = prefab.GetComponent<WaveRenderer>();
        }
        else
        {
            _waveRenderer.gameObject.SetActive(true);
        }

        _playerFreq = 1f;
        _targetFreq = Random.Range(0f, 50f);

        Slider_Wave.value = _playerFreq;

        _waveRenderer.SetTargetWave(_targetFreq);
        _waveRenderer.SetPlayerWave(_playerFreq);
    }

    private bool CheckAnswer()
    {
        if (Mathf.Abs(_playerFreq - _targetFreq) <= _threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
