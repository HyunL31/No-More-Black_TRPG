using System;
using System.Collections.Generic;

public class TextAdventureManager : SingletonBase<TextAdventureManager>
{
    public string CurrentID { get; private set; } = "Dialogue_01";

    public List<KeyValuePair<string, string>> DialogueLogs { get; private set; } = new List<KeyValuePair<string, string>>();

    public Action<string, string> OnAddLog;
    public Action<string> OnChangeBaseUI;
    public Action OnReturnDialogue;

    protected override void Awake()
    {
        base.Awake();
        OnAddLog = AddDialogueLog;
    }

    private void AddDialogueLog(string characterID, string content)
    {
        DialogueLogs.Add(new KeyValuePair<string, string>(characterID, content));
    }

    public void SetCurrentID(string id)
    {
        CurrentID = id;
    }
}
