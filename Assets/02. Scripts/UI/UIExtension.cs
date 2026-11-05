using System;

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
    VisualNovelUI,
    DialogueUI,
    ChoiceUI,
    PolygraphUI,
    InspectorUI,
    DialogueLog,
    GameOverUI,
    InfoUI,
    Background,
    PersonalEventUI,
    GermUI,
    VirusUI,
    WinPanel,
    FailPanel
}

public static class UIExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRoot root, UIType type)
    {
        return $"Prefabs/UI/{root}/{type}";
    }

    public static void OpenBackground(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Background, UIType.Background);
    }

    public static void CloseBackground(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Background, UIType.Background);
    }

    public static void OpenResumeUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.ResumeUI);
    }

    public static void CloseResumeUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.ResumeUI);
    }

    public static void OpenSkillUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.SkillUI);
    }

    public static void CloseSkillUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.SkillUI);
    }

    public static void OpenVisualNovelUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Main, UIType.VisualNovelUI);
    }

    public static void CloseVisualNovelUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Main, UIType.VisualNovelUI);
    }

    public static void OpenDialogueUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.DialogueUI);
    }

    public static void CloseDialogueUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.DialogueUI);
    }

    public static void OpenChoiceUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.ChoiceUI);
    }

    public static void CloseChoiceUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.ChoiceUI);
    }

    public static void OpenPolygraphUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.PolygraphUI);
    }

    public static void ClosePolygraphUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.PolygraphUI);
    }

    public static void OpenInspectorUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.InspectorUI);
    }

    public static void CloseInspectorUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.InspectorUI);
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

    public static void OpenGameOverUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.GameOverUI);
    }

    public static void CloseGameOverUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.GameOverUI);
    }

    public static void OpenInfoUI(this UIManager uiManager, string info, Action accept, bool justInfo = true)
    {
        UIBase uiBase = uiManager.OpenUI(UIRoot.Popup, UIType.InfoUI);

        if (uiBase is InfoUI infoUI)
        {
            infoUI.SetInfoText(info, accept, justInfo);
        }
    }

    public static void CloseInfoUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.InfoUI);
    }

    public static void OpenPersonalEventUI(this UIManager uiManager, string characterID, int num)
    {
        UIBase uiBase = uiManager.OpenUI(UIRoot.Popup, UIType.PersonalEventUI);

        if (uiBase is PersonalEventUI personalEventUI)
        {
            personalEventUI.SetText(characterID, num);
        }
    }

    public static void ClosePersonalEventUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.PersonalEventUI);
    }

    public static void OpenGermUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.GermUI);
    }

    public static void CloseGermUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.GermUI);
    }

    public static void OpenVirusUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Content, UIType.VirusUI);
    }

    public static void CloseVirusUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Content, UIType.VirusUI);
    }

    public static void OpenWinPanel(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.WinPanel);
    }

    public static void CloseWinPanel(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.WinPanel);
    }

    public static void OpenFailPanel(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRoot.Popup, UIType.FailPanel);
    }

    public static void CloseFailPanel(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRoot.Popup, UIType.FailPanel);
    }
}
