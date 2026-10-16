using UnityEngine;
using UnityEngine.UI;

public class DialogueLog : ViewBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Transform Transform_LogParent;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnEnable()
    {
        InitSlot();
    }

    private void OnClickClose()
    {
        UIManager.Inst.CloseDialogueLog();
    }

    private void InitSlot()
    {

    }
}
