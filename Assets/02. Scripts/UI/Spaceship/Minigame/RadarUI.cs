using System.Collections.Generic;
using UnityEngine;

public class RadarUI : ViewBase
{
    [SerializeField] private Transform Transform_Scanner;
    [SerializeField] private Transform Transform_Parent;
    [SerializeField] private RedSpot Prefab_RedSpot;

    private ObjectPooling<RedSpot> _spotPool;
    private List<RedSpot> _activeSpot = new List<RedSpot>();

    private int _targetNum = 10;
    private float _speed = 100f;
    private Vector2 _center;

    private int _removeCount;

    private void Awake()
    {
        _spotPool = new ObjectPooling<RedSpot>(Prefab_RedSpot, Transform_Parent, 10, 30);
        _center = Transform_Parent.position;
    }

    private void OnEnable()
    {
        InitTarget();
        _removeCount = 0;
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

        for (int i = 0; i < _targetNum; i++)
        {
            Vector2 rand = Random.insideUnitCircle * 400;

            RedSpot spot = _spotPool.Get();
            spot.transform.position = _center + rand;

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
}