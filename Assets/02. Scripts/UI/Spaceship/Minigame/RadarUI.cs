using System.Collections.Generic;
using UnityEngine;

public class RadarUI : ViewBase
{
    [SerializeField] private Transform Transform_Scanner;
    [SerializeField] private RectTransform Rect_Frame;
    [SerializeField] private RedSpot Prefab_RedSpot;

    [SerializeField] private Timer Timer;
    [SerializeField] private int Timer_Limit;

    private ObjectPooling<RedSpot> _spotPool;
    private List<RedSpot> _activeSpot = new List<RedSpot>();

    private int _targetNum = 10;
    private float _speed = 100f;
    private Vector2 _center;

    private int _removeCount;

    private void Awake()
    {
        _spotPool = new ObjectPooling<RedSpot>(Prefab_RedSpot, Rect_Frame, 10, 30);
        _center = Rect_Frame.position;
    }

    private void OnEnable()
    {
        InitTarget();
        _removeCount = 0;

        Timer.OnTimeOver += OnTimeOver;
        Timer.StartTimer(Timer_Limit);
    }

    private void OnDisable()
    {
        Timer.OnTimeOver -= OnTimeOver;
    }

    private void Update()
    {
        Transform_Scanner.Rotate(0, 0, -_speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            RemoveSpot();
        }
    }

    private void InitTarget()
    {
        ClearTarget();

        Rect rect = Rect_Frame.rect;

        float radiusX = rect.width * 0.5f;
        float radiusY = rect.height * 0.5f;

        float radius = Mathf.Min(radiusX, radiusY) - 30f;

        for (int i = 0; i < _targetNum; i++)
        {
            RedSpot spot = _spotPool.Get();
            RectTransform spotRect = spot.GetRectSpot();

            float halfWidth = spotRect.rect.width * 0.5f;
            float halfHeight = spotRect.rect.height * 0.5f;

            float maxRadius = radius - Mathf.Max(halfWidth, halfHeight);

            Vector2 randomPos = Random.insideUnitCircle * maxRadius;

            spotRect.anchoredPosition = randomPos;

            _activeSpot.Add(spot);
        }
    }

    private void RemoveSpot()
    {
        for (int i = _activeSpot.Count - 1; i >= 0; i--)
        {
            if (_activeSpot[i] == null)
            {
                _activeSpot.RemoveAt(i);
                continue;
            }

            Vector2 scanDir = Transform_Scanner.up;
            Vector2 dir = (Vector2)_activeSpot[i].transform.position - _center;

            float angle = Vector3.SignedAngle(scanDir, dir, Vector3.forward);

            if (Mathf.Abs(angle) < 5f)
            {
                _spotPool.Release(_activeSpot[i]);

                _activeSpot.RemoveAt(i);
                _removeCount++;
            }

            if (_removeCount == _targetNum)
            {
                UIManager.Inst.OpenWinPanel();
            }
        }
    }

    private void ClearTarget()
    {
        foreach (RedSpot spot in _activeSpot)
        {
            _spotPool.Release(spot);
        }

        _activeSpot.Clear();
    }

    private void OnTimeOver()
    {
        UIManager.Inst.OpenFailPanel();
    }
}