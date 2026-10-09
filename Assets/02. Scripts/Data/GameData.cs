using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataBase
{
    public string ID;
}

[System.Serializable]
public class Character : GameDataBase
{
    public string Name;
    public bool Gender;
    public int Age;
    public float Height;
    public string Major;
    public string RegisterCode;
    public string Job;
    public string Organization;
    public string Command;
    public string Justice;
    public int Stamina;
    public int Crazy;
    public int Oblivion;
    public List<string> Relationship;
    public string MajorCategory;
    public string Career;
    public string Nature;
    public string Wisdom;
    public string Method;
    public string Resistance;
}
