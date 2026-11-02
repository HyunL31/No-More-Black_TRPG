using System;
using UnityEngine;
using UnityEngine.UI;

public class InspectorCharacter : MonoBehaviour
{
    [SerializeField] private CharacterState CharacterState;
    [SerializeField] private Button Button_Character;

    private string _inspectorID = "";
    private bool _isSelected = false;

    private Action<string> _onClickCharacter;

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
        _onClickCharacter?.Invoke(_inspectorID);

        _isSelected = true;

        if (string.IsNullOrEmpty(_inspectorID) == false)
        {
            string[] data = GameDataManager.Inst.GetData<Inspector>(_inspectorID).ClickAnim;
            CharacterState.ApplyEmotion(data[1], data[0]);
        }

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

    private void SetDefaultAnim()
    {
        string[] data = GameDataManager.Inst.GetData<Inspector>(_inspectorID).DefaultAnim;
        CharacterState.ApplyEmotion(data[1], data[0]);
    }

    public void SetInspectorID(string inspectorID)
    {
        _inspectorID = inspectorID;
    }

    public void InitCharacterState()
    {
        SetDefaultAnim();
        CharacterState.SetWhite();
    }

    public void Deselect()
    {
        SetDefaultAnim();

        _isSelected = false;
        SetCharacterColor();
    }

    public void InitAction(Action<string> action)
    {
        _onClickCharacter = action;
    }
}
