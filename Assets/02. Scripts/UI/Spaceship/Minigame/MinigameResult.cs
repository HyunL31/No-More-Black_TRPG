using UnityEngine;
using UnityEngine.UI;

public class MinigameResult : ViewBase
{
    [SerializeField] private Button Button_Close;

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        UIManager.Inst.CloseGermUI();
        UIManager.Inst.CloseVirusUI();

        UIManager.Inst.CloseFailPanel();
        UIManager.Inst.CloseWinPanel();
    }
}