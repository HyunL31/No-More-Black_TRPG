using UnityEngine;
using UnityEngine.UI;

public class InspectorCharacter : MonoBehaviour
{
    [SerializeField] private CharacterState CharacterState;
    [SerializeField] private Button Button_Character;

    private string _inspectorID = "";

    private void Awake()
    {
        Button_Character.onClick.AddListener(OnClickCharacter);
    }

    private void OnClickCharacter()
    {

    }

    public void SetInspectorID(string inspectorID)
    {
        _inspectorID = inspectorID;
    }
}
