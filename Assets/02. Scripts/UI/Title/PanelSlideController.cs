using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PanelSlideController : MonoBehaviour
{
    [SerializeField] private RectTransform Rect_Panel;
    [SerializeField] private float Speed_Duration;
    [SerializeField] private Ease Ease = Ease.OutCubic;

    private Vector2 _homePos;
    private bool _isInit;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_isInit == true)
        {
            return;
        }

        _homePos = Rect_Panel.anchoredPosition;
        _isInit = true;
    }

    private Vector2 GetOffset(Vector2 direction)
    {
        RectTransform parent = Rect_Panel.parent as RectTransform;

        if (direction == Vector2.left || direction == Vector2.right)
        {
            return new Vector2(direction.x * parent.rect.width + 100, direction.y * parent.rect.height);
        }
        else
        {
            return new Vector2(direction.x * parent.rect.width, direction.y * parent.rect.height + 100);
        }
    }

    public void SlideIn(Vector2 fromDirection, Action onComplete = null)
    {
        Init();

        DOTween.Kill(Rect_Panel);
        Rect_Panel.gameObject.SetActive(true);

        Rect_Panel.anchoredPosition = _homePos + GetOffset(fromDirection);

        Rect_Panel.DOAnchorPos(_homePos, Speed_Duration).SetEase(Ease).SetTarget(Rect_Panel).OnComplete(() => onComplete?.Invoke());
    }

    public void SlideOut(Vector2 toDirection, Action onComplete = null)
    {
        Init();

        DOTween.Kill(Rect_Panel);

        Rect_Panel.DOAnchorPos(_homePos + GetOffset(toDirection), Speed_Duration).SetEase(Ease).SetTarget(Rect_Panel).OnComplete(() =>
        {
            Rect_Panel.gameObject.SetActive(false);
            Rect_Panel.anchoredPosition = _homePos;
            onComplete?.Invoke();
        });
    }
}
