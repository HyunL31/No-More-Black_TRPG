using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GermUI : ViewBase
{
    [SerializeField] private Image Image_Quiz;
    [SerializeField] private Button Button_Accept;
    [SerializeField] private Button Button_Reject;

    [SerializeField] private Image Image_Dictionary;
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;
    [SerializeField] private TextMeshProUGUI Text_Name;
    [SerializeField] private TextMeshProUGUI Text_Description;

    private void Awake()
    {
        Button_Accept.onClick.AddListener(OnClickAccept);
        Button_Reject.onClick.AddListener(OnClickReject);
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
    }

    private void OnClickAccept()
    {

    }

    private void OnClickReject()
    {

    }

    private void OnClickLeft()
    {

    }

    private void OnClickRight()
    {

    }
}
