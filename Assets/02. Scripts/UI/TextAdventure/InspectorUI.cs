using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : ViewBase
{
    [SerializeField] private Image Image_Background;
    [SerializeField] private RectTransform Rect_Panorama;
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;
    [SerializeField] private Transform Transform_CharacterContainer;

    private string _inspectorID = "";
    private Dictionary<string, InspectorCharacter> _characters = new Dictionary<string, InspectorCharacter>();

    private void Awake()
    {
        Button_Left.onClick.AddListener(OnClickLeft);
        Button_Right.onClick.AddListener(OnClickRight);
    }

    private void OnEnable()
    {
        _inspectorID = TextAdventureManager.Inst.CurrentID;

        SetBackground().Forget();

        Rect_Panorama.localPosition = new Vector2(960, 0);
        Button_Left.gameObject.SetActive(false);
        Button_Right.gameObject.SetActive(true);

        InitSpineCharacter().Forget();
    }

    private void OnClickLeft()
    {
        if (Button_Right.gameObject.activeSelf == false)
        {
            Button_Right.gameObject.SetActive(true);
        }


    }

    private void OnClickRight()
    {
        if (Button_Left.gameObject.activeSelf == false)
        {
            Button_Left.gameObject.SetActive(true);
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
            }
        }
    }

    private void ClearCharacter()
    {
        foreach (InspectorCharacter character in _characters.Values)
        {
            character.gameObject.SetActive(false);
        }
    }

    private async UniTask SetBackground()
    {
        string background = GameDataManager.Inst.GetData<Inspector>($"{_inspectorID}_1").Background;
        Image_Background.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Background/{background}");
    }
}
