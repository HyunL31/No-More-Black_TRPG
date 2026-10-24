using System;
using UnityEngine;
using UnityEngine.UI;

public class InspectorCharacter : MonoBehaviour
{
    [SerializeField] private CharacterState CharacterState;
    [SerializeField] private Button Button_Character;

    private string _inspectorID = "";
    private bool _isSelected = false;

    private Action _onClickCharacter;

    private void Awake()
    {
        Button_Character.onClick.AddListener(OnClickCharacter);
    }

    private void OnEnable()
    {
        _isSelected = false;
    }

    private void OnClickCharacter()
    {
        _isSelected = true;

        string[] data = GameDataManager.Inst.GetData<Inspector>(_inspectorID).ClickAnim;
        CharacterState.ApplyEmotion(data[1], data[0]);

        SetCharacterColor();
    }

    private void SetCharacterColor()
    {
        if (_isSelected == true)
        {
            CharacterState.SetWhite();
        }
        else
        {
            CharacterState.SetDark();
        }
    }

    public void SetInspectorID(string inspectorID)
    {
        _inspectorID = inspectorID;
    }

    public void InitCharacterState()
    {
        string[] data = GameDataManager.Inst.GetData<Inspector>(_inspectorID).DefaultAnim;

        CharacterState.ApplyEmotion(data[1], data[0]);
        CharacterState.SetWhite();
    }

    public void Deselect()
    {
        _isSelected = false;
        SetCharacterColor();
    }
}
