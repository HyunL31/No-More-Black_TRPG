using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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

    private int _quizNum = 5;
    private int _currentQuiz = 0;
    private List<int> _quizList = new List<int>();

    private List<Germ> _germList = new List<Germ>();
    private int _dictionaryIndex = 0;

    private void Awake()
    {
        Button_Accept.onClick.AddListener(OnClickAccept);
        Button_Reject.onClick.AddListener(OnClickReject);
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
    }

    private void Start()
    {
        SetGermList();
    }

    private void OnEnable()
    {
        SetQuizIndex();
        SetQuizImage(_quizList[_currentQuiz]).Forget();
        SetDictionary(_dictionaryIndex + 1).Forget();
    }

    private void OnClickAccept()
    {
        if (CheckAnswer(_quizList[_currentQuiz]) == false)
        {
            _currentQuiz++;
            SetQuizImage(_quizList[_currentQuiz]).Forget();
        }
        else
        {
            UIManager.Inst.OpenFailPanel();
        }

        if (_currentQuiz >= _quizNum - 1)
        {
            UIManager.Inst.OpenWinPanel();
            return;
        }
    }

    private void OnClickReject()
    {
        if (CheckAnswer(_quizList[_currentQuiz]) == true)
        {
            _currentQuiz++;
            SetQuizImage(_quizList[_currentQuiz]).Forget();
        }
        else
        {
            UIManager.Inst.OpenFailPanel();
        }

        if (_currentQuiz >= _quizNum - 1)
        {
            UIManager.Inst.OpenWinPanel();
            return;
        }
    }

    private void OnClickLeft()
    {
        _dictionaryIndex--;

        if (_dictionaryIndex < 0)
        {
            _dictionaryIndex = _germList.Count - 1;
        }

        SetDictionary(_dictionaryIndex + 1).Forget();
    }

    private void OnClickRight()
    {
        _dictionaryIndex++;

        if (_dictionaryIndex >= _germList.Count)
        {
            _dictionaryIndex = 0;
        }

        SetDictionary(_dictionaryIndex + 1).Forget();
    }

    private void SetGermList()
    {
        _germList = GameDataManager.Inst.GetAllData<Germ>();
    }

    private void SetQuizIndex()
    {
        _quizList.Clear();

        for (int i = 0; i < _quizNum; i++)
        {
            while (true)
            {
                int random = Random.Range(1, 31);

                if (_quizList.Contains(random) == false)
                {
                    _quizList.Add(random);
                    break;
                }
            }
        }
    }

    private async UniTask SetQuizImage(int index)
    {
        string address = $"Sprite/Germ_{index}";
        Sprite sprite = await ResourceManager.Inst.LoadSprite(address);

        Image_Quiz.sprite = sprite;
    }

    private async UniTask SetDictionary(int index)
    {
        string address = $"Sprite/Germ_{index}";

        Image_Dictionary.sprite = await ResourceManager.Inst.LoadSprite(address);

        Text_Name.text = _germList[index - 1].Name;
        Text_Description.text = _germList[index - 1].Description;
    }

    private bool CheckAnswer(int index)
    {
        return _germList[index - 1].Answer;
    }
}