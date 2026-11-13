using UnityEngine;

public class VirusUI : ViewBase
{
    [SerializeField] private Timer Timer;
    [SerializeField] private int Timer_Limit;

    private int _currentAnswer = 0;
    private int _answerNum = 5;

    private void OnEnable()
    {
        _currentAnswer = 0;

        Timer.OnTimeOver += OnTimeOver;
        Timer.StartTimer(Timer_Limit);
    }

    private void OnDisable()
    {
        Timer.OnTimeOver -= OnTimeOver;
    }

    public void AddAnswerCount()
    {
        _currentAnswer++;

        if (_currentAnswer >= _answerNum)
        {
            UIManager.Inst.OpenWinPanel();
        }
    }

    private void OnTimeOver()
    {
        UIManager.Inst.OpenFailPanel();
    }
}
