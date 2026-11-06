using UnityEngine;

public class WaveRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer Line_Player;
    [SerializeField] private LineRenderer Line_Target;

    private int _resolution = 200;
    private float _width = 10f;
    private float _speed = 2f;

    private float _targetFreq;
    private float _playerFreq;

    private float _targetAmp = 1f;
    private float _playerAmp = 1f;

    private void Update()
    {
        float timeOffset = Time.time * _speed;

        DrawWave(Line_Target, _targetFreq, _targetAmp, timeOffset);
        DrawWave(Line_Player, _playerFreq, _playerAmp, timeOffset);
    }

    public void SetTargetWave(float frequency)
    {
        _targetFreq = frequency;
    }

    public void SetPlayerWave(float frequency)
    {
        _playerFreq = frequency;
    }

    private void DrawWave(LineRenderer line, float frequency, float amplitude, float offset)
    {
        line.positionCount = _resolution;

        float ampNoise = 1f + Mathf.Sin(Time.time * 2f) * 0.2f;

        for (int i = 0; i < _resolution; i++)
        {
            float t = (float)i / (_resolution - 1);
            float x = (t - 0.5f) * _width;

            float noise = (Mathf.PerlinNoise(Time.time * 3f, x) - 0.5f) * 0.3f;

            float y = Mathf.Sin(x * frequency - offset) * amplitude * ampNoise + noise;

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
