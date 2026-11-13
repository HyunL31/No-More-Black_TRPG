using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform Rect_Dial;
    [SerializeField] private float Rotation_Sensitivity = 1.5f;

    public event Action<float> OnChangeAngle;

    private float _startAngle;
    private float _currentRotation;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startAngle = GetAngle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float currentAngle = GetAngle(eventData);
        float deltaAngle = Mathf.DeltaAngle(_startAngle, currentAngle);

        _currentRotation -= deltaAngle * Rotation_Sensitivity;
        _currentRotation = Mathf.Clamp(_currentRotation, 0f, 360f);

        Rect_Dial.localRotation = Quaternion.Euler(0, 0, -_currentRotation);

        _startAngle = currentAngle;

        OnChangeAngle?.Invoke(_currentRotation);
    }

    private float GetAngle(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect_Dial, eventData.position, cam, out Vector2 localPoint);

        return Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
    }
}
