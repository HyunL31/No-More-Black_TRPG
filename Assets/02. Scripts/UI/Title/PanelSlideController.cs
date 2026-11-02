using DG.Tweening;
using System;
using UnityEngine;

public class PanelSlideController : MonoBehaviour
{
    [SerializeField] private RectTransform Rect_Panel;
    [SerializeField] private float Speed_Duration;
    [SerializeField] private Ease Ease = Ease.OutCubic;

    [SerializeField] private bool PlayOnEnable = true;
    [SerializeField] private Vector2 Direction_Enter = Vector2.right;
    [SerializeField] private Vector2 Direction_Exit = Vector2.left;

    private Vector2 _homePos;
    private bool _isInit;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if (PlayOnEnable == true)
        {
            SlideIn(Direction_Enter);
        }
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

        if (Rect_Panel.gameObject.activeSelf == false)
        {
            Rect_Panel.gameObject.SetActive(true);
        }

        Rect_Panel.anchoredPosition = _homePos + GetOffset(fromDirection);

        Rect_Panel.DOAnchorPos(_homePos, Speed_Duration).SetEase(Ease).SetTarget(Rect_Panel).OnComplete(() => onComplete?.Invoke());
    }

    public void SlideOut(Vector2 toDirection, Action onComplete = null)
    {
        Init();

        DOTween.Kill(Rect_Panel);

        Rect_Panel.DOAnchorPos(_homePos + GetOffset(toDirection), Speed_Duration).SetEase(Ease).SetTarget(Rect_Panel).OnComplete(() =>
        {
            if (Rect_Panel.gameObject.activeSelf == true && PlayOnEnable == false)
            {
                Rect_Panel.gameObject.SetActive(false);
            }

            Rect_Panel.anchoredPosition = _homePos;
            onComplete?.Invoke();
        });
    }
}
