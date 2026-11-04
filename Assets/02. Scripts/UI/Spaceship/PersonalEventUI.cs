using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalEventUI : ViewBase
{
    [SerializeField] private TextMeshProUGUI Text_RoomNumber;
    [SerializeField] private TextMeshProUGUI Text_CharacterName;
    [SerializeField] private Button Button_Enter;

    private string _characterID;
    private int _eventNum;

    private void Awake()
    {
        Button_Enter.onClick.AddListener(OnClickEnter);
    }

    private void OnClickEnter()
    {
        UIManager.Inst.OpenVisualNovelUI();
        UIManager.Inst.OpenDialogueUI();

        if (SaveManager.Inst.PlayerModel.PersonalEvent.ContainsKey(_characterID) == false)
        {
            SaveManager.Inst.PlayerModel.PersonalEvent.Add(_characterID, new List<int>());
        }

        SaveManager.Inst.PlayerModel.PersonalEvent[_characterID].Add(_eventNum);

        UIManager.Inst.ClosePersonalEventUI();
    }

    public void SetText(string characterID, int num)
    {
        var data = GameDataManager.Inst.GetData<Character>(characterID);
        _characterID = characterID;
        _eventNum = num;

        Text_RoomNumber.text = data.RoomNumber;
        Text_CharacterName.text = data.Name;
    }
}