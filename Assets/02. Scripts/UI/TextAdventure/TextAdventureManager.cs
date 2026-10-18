using System;
using System.Collections.Generic;
using UnityEngine;

public class TextAdventureManager : SingletonBase<TextAdventureManager>
{
    public List<string> DialogueLogs { get; private set; } = new List<string>();

    public Action<string> OnAddLog;
    public Action<string> OnChangeBaseUI;

    protected override void Awake()
    {
        base.Awake();
        OnAddLog = AddDialogueLog;
    }

    private void AddDialogueLog(string contentID)
    {
        DialogueLogs.Add(contentID);
    }
}
