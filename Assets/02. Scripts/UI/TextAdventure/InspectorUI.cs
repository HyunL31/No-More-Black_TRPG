using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : ViewBase
{
    [Header("추론 시스템 UI")]
    [SerializeField] private Image Image_Background;
    [SerializeField] private RectTransform Rect_Panorama;
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;
    [SerializeField] private RectTransform Transform_CharacterContainer;

    [Header("의견 UI")]
    [SerializeField] private GameObject Panel_Info;
    [SerializeField] private PanelSlideController PanelSlideController;
    [SerializeField] private GameObject Panel_Opinion;
    [SerializeField] private TextMeshProUGUI Text_Opinion;
    [SerializeField] private Button Button_Accept;
    [SerializeField] private Button Button_Reject;

    [Header("파노라마")]
    [SerializeField] private float Step_Panorama = 300f;
    [SerializeField] private float Duration_Panorama = 0.3f;
    [SerializeField] private float Offset_Panorama = 800f;

    private float _minX = 960f;
    private float _maxX = 960f;

    private CancellationTokenSource _panoramaToken;

    private string _inspectorID = "";
    private string _currentID = "";
    private Dictionary<string, InspectorCharacter> _characters = new Dictionary<string, InspectorCharacter>();

    private void Awake()
    {
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);

        Button_Reject.onClick.AddListener(OnClickReject);
        Button_Accept.onClick.AddListener(OnClickAccept);
    }

    private void OnEnable()
    {
        _inspectorID = TextAdventureManager.Inst.CurrentID;

        SetBackground().Forget();

        Rect_Panorama.anchoredPosition = new Vector2(960, 0);
        Button_Left.gameObject.SetActive(false);
        Button_Right.gameObject.SetActive(true);

        Panel_Info.gameObject.SetActive(true);
        Panel_Opinion.gameObject.SetActive(false);

        InitSpineCharacter().Forget();
    }

    private void OnClickLeft()
    {
        float targetX = Mathf.Clamp(Rect_Panorama.anchoredPosition.x + Step_Panorama, _minX, _maxX);
        MovePanorama(targetX).Forget();
    }

    private void OnClickRight()
    {
        float targetX = Mathf.Clamp(Rect_Panorama.anchoredPosition.x - Step_Panorama, _minX, _maxX);
        MovePanorama(targetX).Forget();
    }

    private void OnClickReject()
    {
        PanelSlideController.SlideOut(Vector2.left);

        Panel_Opinion.SetActive(false);
        Panel_Info.SetActive(true);

        InitCharacterState();
    }

    private void OnClickAccept()
    {
        var data = GameDataManager.Inst.GetData<Inspector>(_currentID);
        TextAdventureManager.Inst.SetCurrentID(data.NextID);
        TextAdventureManager.Inst.OnAddLog(data.CharacterID, data.Content);

        UIManager.Inst.OpenVisualNovelUI();
        UIManager.Inst.OpenDialogueUI();
        UIManager.Inst.CloseInspectorUI();
    }

    private void UpdateButtons()
    {
        float currentX = Rect_Panorama.anchoredPosition.x;

        Button_Left.gameObject.SetActive(currentX < _maxX);
        Button_Right.gameObject.SetActive(currentX > _minX);
    }

    private async UniTaskVoid MovePanorama(float targetX)
    {
        CancelPanorama();
        _panoramaToken = new CancellationTokenSource();

        Vector2 startPos = Rect_Panorama.anchoredPosition;
        Vector2 endPos = new Vector2(targetX, startPos.y);

        float elapsed = 0f;

        while (elapsed < Duration_Panorama)
        {
            if (_panoramaToken.IsCancellationRequested)
            {
                return;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Duration_Panorama);

            t = Mathf.SmoothStep(0f, 1f, t);

            Rect_Panorama.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            await UniTask.Yield(PlayerLoopTiming.Update, _panoramaToken.Token);
        }

        Rect_Panorama.anchoredPosition = endPos;
        UpdateButtons();
    }

    private void CancelPanorama()
    {
        if (_panoramaToken != null)
        {
            _panoramaToken.Cancel();
            _panoramaToken.Dispose();
            _panoramaToken = null;
        }
    }

    private async UniTask InitSpineCharacter()
    {
        ClearCharacter();

        var data = GameDataManager.Inst.GetDataList<Inspector>();

        foreach (Inspector inspector in data)
        {
            string characterID = inspector.CharacterID;

            if (inspector.ID.Contains(_inspectorID))
            {
                if (_characters.ContainsKey(characterID) == false)
                {
                    GameObject prefab = await ResourceManager.Inst.InstantiateAsync($"Spine/{characterID}", Transform_CharacterContainer);
                    InspectorCharacter character = prefab.GetComponent<InspectorCharacter>();
                    character.enabled = true;
                    _characters.Add(characterID, character);
                }
                else
                {
                    _characters[characterID].gameObject.SetActive(true);
                }

                _characters[characterID].SetInspectorID(inspector.ID);
                _characters[characterID].InitCharacterState();
                _characters[characterID].InitAction((inspectorID) => ClearCharacterState(inspectorID));
            }
        }

        await UniTask.Yield();

        float contentWidth = Transform_CharacterContainer.rect.width;
        _minX = 960f - contentWidth + Offset_Panorama;
        _maxX = 960f;
    }

    private void ClearCharacter()
    {
        foreach (InspectorCharacter character in _characters.Values)
        {
            character.gameObject.SetActive(false);
        }
    }

    private void ClearCharacterState(string inspectorID = "")
    {
        foreach (InspectorCharacter character in _characters.Values)
        {
            character.Deselect();
        }

        _currentID = inspectorID;
        
        if (string.IsNullOrEmpty(_currentID) == false)
        {
            SetOpinionText();

            Panel_Info.SetActive(false);
            Panel_Opinion.SetActive(true);
            PanelSlideController.SlideIn(Vector2.left);
        }
    }

    private void InitCharacterState()
    {
        foreach (InspectorCharacter character in _characters.Values)
        {
            character.InitCharacterState();
        }
    }

    private async UniTask SetBackground()
    {
        string background = GameDataManager.Inst.GetData<Inspector>($"{_inspectorID}_1").Background;
        Image_Background.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Background/{background}");
    }

    private void SetOpinionText()
    {
        string content = GameDataManager.Inst.GetData<Inspector>(_currentID).Content;
        Text_Opinion.text = content;
    }
}
