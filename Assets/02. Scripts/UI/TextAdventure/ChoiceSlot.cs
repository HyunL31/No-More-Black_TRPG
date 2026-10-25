using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSlot : MonoBehaviour
{
    [SerializeField] private Button Button_Choice;
    [SerializeField] private TextMeshProUGUI Text_Choice;

    private string[] _resultID;
    private string _returnID = "";

    private void Awake()
    {
        Button_Choice.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        foreach (string resultID in _resultID)
        {
            TextAdventureManager.Inst.ApplyResult(resultID);
        }

        TextAdventureManager.Inst.SetCurrentID(_returnID);
        TextAdventureManager.Inst.OnAddLog?.Invoke("Player_01", Text_Choice.text);

        UIManager.Inst.OpenDialogueUI();
        TextAdventureManager.Inst.OnReturnDialogue?.Invoke();

        UIManager.Inst.CloseChoiceUI();
    }

    public void SetSlot(string content)
    {
        Text_Choice.text = content;
    }

    public void SetReturnID(string[] resultID, string returnID)
    {
        _resultID = resultID;
        _returnID = returnID;
    }
}
