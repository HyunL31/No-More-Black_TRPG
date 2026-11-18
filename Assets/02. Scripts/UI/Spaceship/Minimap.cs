using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private RectTransform Parent_Clue;
    [SerializeField] private ClueMarker Prefab_ClueSpot;

    [SerializeField] private RectTransform Rect_Player;

    [SerializeField] private float Radius_View = 20f;
    [SerializeField] private float Interval_Update = 0.1f;

    private PlayerController _player;

    private ObjectPooling<ClueMarker> _cluePool;
    private List<ClueMarker> _activeMarker = new List<ClueMarker>();

    private float _timer;

    private PlayerDirection _lastDirection;
    private bool _lastFacingRight;
    private float _mapRadius;

    private void Awake()
    {
        _cluePool = new ObjectPooling<ClueMarker>(Prefab_ClueSpot, Parent_Clue, 10, 30);
    }

    private void Start()
    {
        _mapRadius = Mathf.Min(Parent_Clue.rect.width, Parent_Clue.rect.height) * 0.5f;

        UpdatePlayerArrow(true);
        UpdateCluePosition();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < Interval_Update)
        {
            return;
        }

        _timer = 0f;

        UpdatePlayerArrow();
        UpdateCluePosition();
    }

    public void RegisterClue(Transform target)
    {
        if (target == null)
        {
            return;
        }

        for (int i = 0; i < _activeMarker.Count; i++)
        {
            if (_activeMarker[i] != null && _activeMarker[i].WorldTarget == target)
            {
                return;
            }
        }

        ClueMarker marker = _cluePool.Get();

        marker.SetTarget(target);

        _activeMarker.Add(marker);
    }

    public void UnregisterClue(Transform target)
    {
        for (int i = _activeMarker.Count - 1; i >= 0; i--)
        {
            ClueMarker marker = _activeMarker[i];

            if (marker == null)
            {
                _activeMarker.RemoveAt(i);
                continue;
            }

            if (marker.WorldTarget != target)
            {
                continue;
            }

            _activeMarker.RemoveAt(i);

            marker.Clear();
            _cluePool.Release(marker);

            break;
        }
    }

    private void UpdatePlayerArrow(bool force = false)
    {
        if (_player == null)
        {
            _player = GameObjectManager.Inst.GetPlayer();
        }

        PlayerDirection direction = _player.CurrentDirection;
        bool facingRight = _player.IsRight;

        if (force == false && direction == _lastDirection && facingRight == _lastFacingRight)
        {
            return;
        }

        Vector3 facing = _player.GetFacingDirection();

        float angle = -Mathf.Atan2(facing.x, facing.z) * Mathf.Rad2Deg;

        Rect_Player.localRotation = Quaternion.Euler(0f, 0f, angle);

        _lastDirection = direction;
        _lastFacingRight = facingRight;
    }

    private void UpdateCluePosition()
    {
        Vector3 playerPosition = _player.transform.position;

        float radiusSqr = Radius_View * Radius_View;
        float mapRadius = _mapRadius;

        for (int i = _activeMarker.Count - 1; i >= 0; i--)
        {
            ClueMarker marker = _activeMarker[i];

            if (marker == null || marker.WorldTarget == null)
            {
                if (marker != null)
                {
                    marker.Clear();
                    _cluePool.Release(marker);
                }

                _activeMarker.RemoveAt(i);
                continue;
            }

            Vector3 offset = marker.WorldTarget.position - playerPosition;
            Vector2 flatOffset = new Vector2(offset.x, offset.z);

            bool inRange = flatOffset.sqrMagnitude <= radiusSqr;

            marker.SetVisible(inRange);

            if (inRange == false)
            {
                continue;
            }

            Vector2 uiPosition = flatOffset / Radius_View * mapRadius;

            marker.SetPosition(uiPosition);
        }
    }
}
