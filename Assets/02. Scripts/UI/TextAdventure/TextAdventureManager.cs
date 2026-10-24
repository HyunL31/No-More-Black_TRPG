using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    None,
    Stamina,
    Crazy,
    Oblivion,
    Skill
}

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

    public void ApplyResult(string resultID)
    {
        GameUtil.ApplyResult(resultID);
    }

    public void SetCurrentID(string id)
    {
        CurrentID = id;
    }
}
