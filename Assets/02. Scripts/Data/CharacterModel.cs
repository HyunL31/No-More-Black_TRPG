using System;
using System.Collections.Generic;

[Serializable]
public class Relation
{
    public string TargetID;
    public RelationType Type = RelationType.Neutral;

    public Relation(string targetID, RelationType type)
    {
        TargetID = targetID;
        Type = type;
    }
}

[Serializable]
public class CharacterModel
{
    public string ID;
    public bool IsPlayer;

    // 기본 정보
    public string Name;
    public string RegisterCode;
    public string Job;
    public bool IsGirl;
    public int Age;
    public float Height;

    public string Command;
    public string Justice;

    public string Major;
    public string Organization;

    // 운영
    public int Stamina;
    public int Crazy;
    public int Oblivion;
    public int Ideology;

    // 인연
    public List<Relation> Relations = new List<Relation>();

    // 역량
    public List<string> SelectedMajor = new List<string>();
    public List<string> SelectedCareer = new List<string>();
    public string SelectedNature = "";
    public List<string> SelectedWisdom = new List<string>();
    public List<string> SelectedMethod = new List<string>();
    public List<string> SelectedResistance = new List<string>();

    public List<SkillCategory> CategoryUnlock = new List<SkillCategory>();

    public List<string> AbandonedSkillID = new List<string>();

    public CharacterModel(Character character, bool isPlayer = false)
    {
        ID = character.ID;
        IsPlayer = isPlayer;
        Name = character.Name;
        RegisterCode = character.RegisterCode;
        Job = character.Job;
        IsGirl = character.Gender;
        Age = character.Age;
        Height = character.Height;
        Command = character.Command;
        Justice = character.Justice;
        Major = character.Major;
        Organization = character.Organization;
        Stamina = character.Stamina;
        Crazy = character.Crazy;
        Oblivion = character.Oblivion;
        Relations = character.GetRelations();

        AddList(SelectedMajor, character.MajorCategory);
        AddList(SelectedCareer, character.Career);
        AddList(SelectedWisdom, character.Wisdom);
        AddList(SelectedMethod, character.Method);
        AddList(SelectedResistance, character.Resistance);

        SelectedNature = character.Nature;

        CategoryUnlock.Add(SkillCategory.Nature);
    }

    private static void AddList(List<string> list, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (!list.Contains(value))
        {
            list.Add(value);
        }
    }

    public Relation GetRelation(string targetID, RelationType type)
    {
        if (Relations == null)
        {
            return null;
        }

        for (int i = 0; i < Relations.Count; i++)
        {
            Relation relation = Relations[i];

            if (relation != null && relation.TargetID == targetID && relation.Type == type)
            {
                return relation;
            }
        }

        return null;
    }

    public void SetRelation(string targetID, RelationType type)
    {
        Relation info = GetRelation(targetID, type);

        if (info == null)
        {
            bool hasPact = false;
            string pactID = null;

            foreach (Relation relation in Relations)
            {
                if (relation.Type == RelationType.Pact)
                {
                    hasPact = true;
                    pactID = relation.TargetID;

                    break;
                }
            }

            if (type == RelationType.Pact && hasPact)
            {
                Relations.Add(new Relation(pactID, RelationType.Friendship));
                RemoveRelation(pactID, RelationType.Pact);
            }

            Relations.Add(new Relation(targetID, type));
        }
    }

    public bool RemoveRelation(string targetID, RelationType type)
    {
        Relation relation = GetRelation(targetID, type);

        if (relation == null)
        {
            return false;
        }

        Relations.Remove(relation);

        return true;
    }
}

public enum RelationType
{
    None,
    Hostile,
    Friction,
    Neutral,
    Friendly,
    Friendship,
    Pact
}

public enum SkillCategory
{
    None,
    Major,
    Career,
    Nature,
    Wisdom,
    Method,
    Resistance
}