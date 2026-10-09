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
    ResumeUI
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
}
