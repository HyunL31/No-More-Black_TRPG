using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalEventUI : ViewBase
{
    [SerializeField] private TextMeshProUGUI Text_RoomNumber;
    [SerializeField] private TextMeshProUGUI Text_CharacterName;
    [SerializeField] private Button Button_Enter;

    private void Awake()
    {
        Button_Enter.onClick.AddListener(OnClickEnter);
    }

    private void OnClickEnter()
    {
        UIManager.Inst.OpenVisualNovelUI();
        UIManager.Inst.OpenDialogueUI();
        UIManager.Inst.ClosePersonalEventUI();
    }

    private void SetText()
    {
        
    }
}