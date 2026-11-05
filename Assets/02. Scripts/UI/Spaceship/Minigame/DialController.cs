using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform Rect_Dial;

    public event Action<float> OnChangeAngle;

    private float _startAngle;
    private float _currentRotation;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float currentAngle = GetAngle(eventData.position);
        float deltaAngle = Mathf.DeltaAngle(_startAngle, currentAngle);
        float snappedDelta = Mathf.Round(deltaAngle / 3f) * 3f;

        if (snappedDelta == 0f)
        {
            return;
        }

        _currentRotation -= snappedDelta;
        _currentRotation = Mathf.Clamp(_currentRotation, 0f, 360f);

        Rect_Dial.localRotation = Quaternion.Euler(0, 0, -_currentRotation);

        _startAngle = currentAngle;

        OnChangeAngle?.Invoke(_currentRotation);
    }

    private float GetAngle(Vector2 screenPos)
    {
        Vector2 dir = screenPos - (Vector2)Rect_Dial.position;

        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
