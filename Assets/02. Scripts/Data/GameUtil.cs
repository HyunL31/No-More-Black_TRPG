using System;
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
}