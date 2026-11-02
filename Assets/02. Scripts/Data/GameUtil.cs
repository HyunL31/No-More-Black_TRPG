using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class GameUtil
{
    public static bool Compare(float a, float b, string operation)
    {
        switch (operation)
        {
            case "<":
                return a < b;

            case ">":
                return a > b;

            case ">=":
                return a >= b;

            case "<=":
                return a <= b;

            default:
                return a == b;
        }
    }

    //public static string GetPopupMessage(string key, params object[] args)
    //{
    //    var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(key);

    //    return string.Format(localizedString, args);
    //}

    public static string GetPopupMessage(string format, params object[] args)
    {
        return string.Format(format, args);
    }

    public static string GetRelationText(RelationType relation)
    {
        switch (relation)
        {
            case RelationType.Hostile: return "적대";
            case RelationType.Friction: return "불화";
            case RelationType.Neutral: return "중립";
            case RelationType.Friendly: return "우호";
            case RelationType.Friendship: return "우정";
            case RelationType.Pact: return "맹약";
            default: return "중립";
        }
    }
}