using Cysharp.Threading.Tasks;
using Spine;
using System;
using System.Collections.Generic;
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

    [SerializeField] private Button Button_Log;
    [SerializeField] private Button Button_Auto;

    [Header("캐릭터 슬롯")]
    [SerializeField] private Transform Transform_Left;
    [SerializeField] private Transform Transform_LeftMiddle;
    [SerializeField] private Transform Transform_Middle;
    [SerializeField] private Transform Transform_RightMiddle;
    [SerializeField] private Transform Transform_Right;

    private bool _isAuto = false;
    private float _autoWait = 0.5f;

    private string _currentID = "Dialogue_01";
    private float _typingSpeed = 0.05f;
    private bool _isTyping = false;
    private CancellationTokenSource _typingToken;

    private Dictionary<string, CharacterState> _characters = new Dictionary<string, CharacterState>();

    private void Awake()
    {
        Button_Dialogue.onClick.AddListener(OnClickDialogueButton);
        Button_Skip.onClick.AddListener(OnClickSkip);
        Button_Auto.onClick.AddListener(OnClickAuto);
        Button_Log.onClick.AddListener(OnClickLog);
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
            ShowDialogue();
        }
    }

    private void OnClickLog()
    {
        UIManager.Inst.OpenDialogueLog();
    }

    private void OnClickAuto()
    {
        _isAuto = !_isAuto;

        if (_isTyping == false && _isAuto == true)
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

        string speaker = GameDataManager.Inst.GetData<Character>(data.Speaker).Name;
        Text_Speaker.text = speaker;

        ChangeBackground(data.Background).Forget();

        CancelTyping();
        _typingToken = new CancellationTokenSource();
        TypingEffect(data.Content, _typingToken.Token).Forget();

        SetBGM(data.BGM);
        SetSFX(data.SFX);

        ClearCharacter();

        string[] characters = data.Character;
        string[] slots = data.Slot;
        string[] emotions = data.Emotion;
        string[] animations = data.Animation;

        for (int i = 0; i < characters.Length; i++)
        {
            bool isSpeaker = i == 0 ? true : false;

            ChangeCharacter(characters[i], slots[i], animations[i], emotions[i], isSpeaker).Forget();
        }

        TextAdventureManager.Inst.OnAddLog?.Invoke(_currentID);

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

        if (_isAuto == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_autoWait), cancellationToken: token);

            ShowDialogue();
        }
    }

    private async UniTask ChangeBackground(string background)
    {
        Image_Background.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Background/{background}");
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

    private async UniTask ChangeCharacter(string characterID, string slot, string animation, string skin, bool isSpeaker)
    {
        GameObject prefab = null;

        if (_characters.ContainsKey(characterID) == false)
        {
            prefab = await ResourceManager.Inst.InstantiateAsync($"Spine/{characterID}", GetTransform(slot));
            CharacterState characterState = prefab.GetComponent<CharacterState>();

            _characters.Add(characterID, characterState);
        }
        else
        {
            prefab = _characters[characterID].gameObject;
            prefab.SetActive(true);
            prefab.transform.SetParent(GetTransform(slot));
        }

        _characters[characterID].ApplyEmotion(animation, skin);

        if (isSpeaker == true)
        {
            _characters[characterID].SetWhite();
        }
        else
        {
            _characters[characterID].SetDark();
        }
    }

    private void ClearCharacter()
    {
        foreach (CharacterState state in _characters.Values)
        {
            state.gameObject.SetActive(false);
        }
    }

    private Transform GetTransform(string slot)
    {
        switch (slot)
        {
            case "L":
                return Transform_Left;

            case "LM":
                return Transform_LeftMiddle;

            case "M":
                return Transform_Middle;

            case "RM":
                return Transform_RightMiddle;

            case "R":
                return Transform_Right;
        }

        return transform;
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
