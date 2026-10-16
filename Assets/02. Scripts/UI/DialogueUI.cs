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
    [SerializeField] private Image Image_Background;

    private string _currentID = "Dialogue_01";
    private float _typingSpeed = 0.05f;
    private bool _isTyping = false;
    private CancellationTokenSource _typingToken;

    private void Awake()
    {
        Button_Dialogue.onClick.AddListener(OnClickDialogueButton);
        Button_Skip.onClick.AddListener(OnClickSkip);
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
            ShowDialogue();
        }
    }

    private void OnClickSkip()
    {

    }

    private void ShowDialogue()
    {
        Image_NextArrow.gameObject.SetActive(false);

        var data = GameDataManager.Inst.GetData<Dialogue>(_currentID);

        Text_Speaker.text = GameDataManager.Inst.GetData<Character>(data.Speaker).Name;

        ChangeBackground(data.Background).Forget();

        CancelTyping();
        _typingToken = new CancellationTokenSource();
        TypingEffect(data.Content, _typingToken.Token).Forget();

        SetBGM(data.BGM);
        SetSFX(data.SFX);

        _currentID = data.NextID;
    }

    private void MoveToNextID()
    {

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
    }

    private async UniTask ChangeBackground(string background)
    {
        Image_Background.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Background/{background}");
    }

    private void SetBGM(string bgm)
    {
        if (bgm != null)
        {
            SoundManager.Inst.PlayBGM(bgm);
        }
    }

    private void SetSFX(string sfx)
    {
        if (sfx != null)
        {
            SoundManager.Inst.PlaySFX(sfx);
        }
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
}
