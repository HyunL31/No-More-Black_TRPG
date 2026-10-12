using UnityEngine;
using UnityEngine.UI;

public class GuageSlot : MonoBehaviour
{
    [SerializeField] private Image Image_Filled;

    public void SetActiveImage(bool isActive, Color color)
    {
        Image_Filled.gameObject.SetActive(isActive);
        Image_Filled.color = color;
    }
}