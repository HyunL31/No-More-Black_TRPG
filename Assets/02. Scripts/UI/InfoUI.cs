using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : ViewBase
{
    [SerializeField] private TextMeshProUGUI Text_Info;

    [SerializeField] private Button Button_Confirm;
    [SerializeField] private GameObject Panel_Buttons;
    [SerializeField] private Button Button_Accept;
    [SerializeField] private Button Button_Reject;

    private Action _onAccept;

    private void Awake()
    {
        Button_Accept.onClick.AddListener(OnClickAccept);
        Button_Reject.onClick.AddListener(UIManager.Inst.CloseInfoUI);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnClickAccept()
    {
        _onAccept?.Invoke();
        UIManager.Inst.CloseInfoUI();
    }

    private void OnClickConfirm()
    {
        _onAccept?.Invoke();
        UIManager.Inst.CloseInfoUI();
    }

    public void SetInfoText(string info, Action accept, bool justInfo = true)
    {
        Text_Info.text = info;

        Panel_Buttons.SetActive(!justInfo);
        Button_Confirm.gameObject.SetActive(justInfo);

        _onAccept = accept;
    }
}
