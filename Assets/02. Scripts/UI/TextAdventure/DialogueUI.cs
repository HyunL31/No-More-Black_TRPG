using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : ViewBase
{
    [SerializeField] private Button Button_Dialogue;
    [SerializeField] private TextMeshProUGUI Text_Dialogue;
    [SerializeField] private TextMeshProUGUI Text_Speaker;
    [SerializeField] private Button Button_Skip;
    [SerializeField] private Image Image_NextArrow;

    [SerializeField] private Button Button_Log;
    [SerializeField] private Button Button_Auto;

    private bool _isAuto = false;
    private float _autoWait = 0.5f;

    private float _typingSpeed = 0.05f;
    private bool _isTyping = false;
    private CancellationTokenSource _typingToken;

    private void Awake()
    {
        Button_Dialogue.onClick.AddListener(OnClickDialogueButton);
        Button_Skip.onClick.AddListener(OnClickSkip);
        Button_Auto.onClick.AddListener(OnClickAuto);
        Button_Log.onClick.AddListener(OnClickLog);

        TextAdventureManager.Inst.OnReturnDialogue += ShowDialogue;
    }

    private void OnEnable()
    {
        ShowDialogue();
    }

    private void OnDestroy()
    {
        CancelTyping();
    }

    private void OnClickDialogueButton()
    {
        if (_isTyping == true)
        {
            _isTyping = false;
        }
        else
        {
            MoveToNextContent();
        }
    }

    private void OnClickLog()
    {
        UIManager.Inst.OpenDialogueLog();
    }

    private void OnClickAuto()
    {
        _isAuto = !_isAuto;

        if (_isTyping == false && _isAuto == true && CanNext() == true)
        {
            ShowDialogue();
        }
    }

    private void OnClickSkip()
    {
        CancelTyping();

        bool isPlaying = false;

        while (isPlaying == false)
        {
            Debug.Log(GetCurrentID());
            var data = GameDataManager.Inst.GetData<Dialogue>(GetCurrentID());
            string nextID = data.NextID;

            if (nextID.Contains("Choice") || nextID == "0")
            {
                isPlaying = true;
                break;
            }

            TextAdventureManager.Inst.OnAddLog?.Invoke(data.Speaker, data.Content);
            TextAdventureManager.Inst.SetCurrentID(nextID);
        }

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        string currentID = GetCurrentID();

        Image_NextArrow.gameObject.SetActive(false);

        var data = GameDataManager.Inst.GetData<Dialogue>(currentID);

        string speaker = GameDataManager.Inst.GetData<Character>(data.Speaker).Name;
        Text_Speaker.text = speaker;

        CancelTyping();
        _typingToken = new CancellationTokenSource();
        TypingEffect(data.Content, _typingToken.Token).Forget();

        SetBGM(data.BGM);
        SetSFX(data.SFX);

        TextAdventureManager.Inst.OnChangeBaseUI?.Invoke(currentID);
        TextAdventureManager.Inst.OnAddLog?.Invoke(data.Speaker, data.Content);

        TextAdventureManager.Inst.SetCurrentID(data.NextID);
    }

    private void MoveToNextContent()
    {
        if (GetCurrentID().Contains("Dialogue"))
        {
            ShowDialogue();
        }
        else if(GetCurrentID().Contains("Choice"))
        {
            UIManager.Inst.OpenChoiceUI();
        }
        else if (GetCurrentID().Contains("Poly"))
        {
            UIManager.Inst.OpenPolygraphUI();
            UIManager.Inst.CloseDialogueUI();
        }
    }

    private async UniTask TypingEffect(string content, CancellationToken token)
    {
        _isTyping = true;

        Text_Dialogue.text = content;
        Text_Dialogue.maxVisibleCharacters = 0;

        while (Text_Dialogue.maxVisibleCharacters < content.Length)
        {
            if (_isTyping == false)
            {
                break;
            }

            Text_Dialogue.maxVisibleCharacters++;

            await UniTask.Delay(TimeSpan.FromSeconds(_typingSpeed), cancellationToken : token);
        }

        Text_Dialogue.maxVisibleCharacters = content.Length;
        _isTyping = false;
        Image_NextArrow.gameObject.SetActive(true);

        if (_isAuto == true && CanNext() == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_autoWait), cancellationToken: token);

            ShowDialogue();
        }
    }

    private void SetBGM(string bgm)
    {
        if (string.IsNullOrEmpty(bgm) == false)
        {
            SoundManager.Inst.PlayBGM(bgm);
        }
    }

    private void SetSFX(string sfx)
    {
        if (string.IsNullOrEmpty(sfx) == false)
        {
            SoundManager.Inst.PlaySFX(sfx);
        }
    }

    private string GetCurrentID()
    {
        return TextAdventureManager.Inst.CurrentID;
    }

    private void CancelTyping()
    {
        if (_typingToken != null)
        {
            _typingToken.Cancel();
            _typingToken.Dispose();
            _typingToken = null;
        }
    }

    private bool CanNext()
    {
        if (UIManager.Inst.IsOpend(UIType.DialogueLog) == false && UIManager.Inst.IsOpend(UIType.ChoiceUI) == false)
        {
            return true;
        }

        return false;
    }
}
