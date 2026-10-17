using Unity.VisualScripting;
using UnityEngine;

public enum UIRoot
{
    None,
    Background,
    Main,
    Content,
    Popup,
    Front
}

public enum UIType
{
    None,
    TitleUI,
    ResumeUI,
    SkillUI,
    DialogueUI,
    DialogueLog
}

public static class UIExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRoot root, UIType type)
    {
        return $"Prefabs/UI/{root}/{type}";
    }

    public static void OpenResumeUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.ResumeUI);
    }

    public static void CloseResumeUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.ResumeUI);
    }

    public static void OpenSkillUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.SkillUI);
    }

    public static void CloseSkillUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.SkillUI);
    }

    public static void OpenDialogueUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Main, UIType.DialogueUI);
    }

    public static void CloseDialogueUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Main, UIType.DialogueUI);
    }

    public static void OpenDialogueLog(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.DialogueLog);
    }

    public static void CloseDialogueLog(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.DialogueLog);
    }

    public static void OpenTitleUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Main, UIType.TitleUI);
    }

    public static void CloseTitleUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Main, UIType.TitleUI);
    }
}
