using UnityEngine;

public class RedSpot : MonoBehaviour
{
    [SerializeField] RectTransform Rect_Spot;

    public RectTransform GetRectSpot()
    {
        return Rect_Spot;
    }
}
