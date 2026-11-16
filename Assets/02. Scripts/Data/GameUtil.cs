using UnityEngine;

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

    public static string GetIdeologyName(float ideology)
    {
        if (ideology == 0)
        {
            return "균형";
        }
        else
        {
            if (ideology <= -3)
            {
                return "혁명";
            }
            else if (ideology == -2)
            {
                return "개혁";
            }
            else if (ideology == -1)
            {
                return "발전";
            }
            else if (ideology == 1)
            {
                return "안정";
            }
            else if (ideology == 2)
            {
                return "헌신";
            }
            else
            {
                return "수호";
            }
        }
    }

    public static int Roll(int side)
    {
        return Random.Range(1, side + 1);
    }

    public static int RollNormal()
    {
        return Roll(20);
    }

    public static int RollFail()
    {
        return Roll(3);
    }

    public static int RollSpecial()
    {
        return Mathf.Min(Roll(20), Roll(20));
    }
}