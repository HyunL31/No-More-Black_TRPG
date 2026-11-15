using UnityEngine;

public class ClueMarker : MonoBehaviour
{
    [SerializeField] private RectTransform Rect_Clue;

    public Transform WorldTarget {  get; private set; }

    public void SetTarget(Transform target)
    {
        WorldTarget = target;
        Rect_Clue.anchoredPosition = Vector2.zero;

        SetVisible(target != null);
    }

    public void SetPosition(Vector2 position)
    {
        Rect_Clue.anchoredPosition = position;
    }

    public void SetVisible(bool isVisible)
    {
        if (gameObject.activeSelf != isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }

    public void Clear()
    {
        WorldTarget = null;
        Rect_Clue.anchoredPosition = Vector2.zero;

        SetVisible(false);
    }
}
