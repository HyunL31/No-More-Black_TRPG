using UnityEngine;
using UnityEngine.EventSystems;

public class TextMouseHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private ScrollSelector ScrollSelector;
    [SerializeField] private int _currentIndex = 0;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ScrollSelector.SelectButton(_currentIndex);
    }
}
