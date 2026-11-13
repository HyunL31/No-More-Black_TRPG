using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirusSlot : MonoBehaviour
{
    [SerializeField] private Button Button_Reject;
    [SerializeField] private TextMeshProUGUI Text_Log;

    private Action _onAddAnswer;

    private string _virusID;

    private Color _selectColor = Color.red;

    private void Awake()
    {
        Button_Reject.onClick.AddListener(OnClickReject);
    }

    private void OnDisable()
    {
        _onAddAnswer = null;
    }

    private void OnClickReject()
    {
        if (CheckAnswer() == false)
        {
            UIManager.Inst.OpenFailPanel();
        }
        else
        {
            Text_Log.color = _selectColor;
            _onAddAnswer?.Invoke();
        }
    }

    private bool CheckAnswer()
    {
        return GameDataManager.Inst.GetData<Virus>(_virusID).Answer;
    }

    public void SetText(Action onAnswer)
    {
        Text_Log.text = GameDataManager.Inst.GetData<Virus>(_virusID).Content;

        _onAddAnswer = onAnswer;
    }

    public void SetVirusID(string virusID)
    {
        _virusID = virusID;
    }
}
