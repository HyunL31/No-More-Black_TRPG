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
    None
}

public static class UIExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRoot root, UIType type)
    {
        return $"Prefabs/UI/{root}/{type}";
    }
}
