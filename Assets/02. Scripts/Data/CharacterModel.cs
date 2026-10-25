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
    public List<MajorType> SelectedMajor = new List<MajorType>();
    public List<CareerType> SelectedCareer = new List<CareerType>();
    public NatureType SelectedNature = NatureType.None;
    public List<WisdomType> SelectedWisdom = new List<WisdomType>();
    public List<MethodType> SelectedMethod = new List<MethodType>();
    public List<ResistanceType> SelectedResistance = new List<ResistanceType>();

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

        AddParsed(SelectedMajor, character.MajorCategory);
        AddParsed(SelectedCareer, character.Career);
        AddParsed(SelectedWisdom, character.Wisdom);
        AddParsed(SelectedMethod, character.Method);
        AddParsed(SelectedResistance, character.Resistance);

        if (Enum.TryParse(character.Nature, out NatureType result))
        {
            SelectedNature = result;
        }

        CategoryUnlock.Add(SkillCategory.Nature);
    }

    private static void AddParsed<T>(List<T> list, string value) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (Enum.TryParse<T>(value, out var parsed) && !list.Contains(parsed))
        {
            list.Add(parsed);
        }
    }

    public Relation GetRelation(string targetID)
    {
        if (Relations == null)
        {
            return null;
        }

        for (int i = 0; i < Relations.Count; i++)
        {
            Relation relation = Relations[i];

            if (relation != null && relation.TargetID == targetID)
            {
                return relation;
            }
        }

        return null;
    }

    public void SetRelation(string targetId, RelationType type)
    {
        Relation info = GetRelation(targetId);

        if (info != null)
        {
            info.Type = type;
        }
        else
        {
            Relations.Add(new Relation(targetId, type));
        }
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

public enum MajorType
{
    None,
    Medical,
    Physics,
    Astronomy,
    Chemistry,
    Biology,
    History,
    Economics,
    Religion,
    Arts,
    Language,
    Humanities,
    Engineering
}

public enum CareerType
{
    None,
    Repair,
    Manufacturing,
    Information,
    Medical,
    Agriculture,
    Regeneration,
    Construction,
    Education,
    Law,
    Administration,
    Driving,
    Finance
}

public enum NatureType
{
    None,
    Selfish,
    Fragile,
    Stubborn,
    Vanity,
    Suspicion,
    Impulsive,
    Pessimistic,
    Hypocrisy,
    Lethargy,
    Greedy,
    Control,
    Cynical
}

public enum WisdomType
{
    None,
    Negotiation,
    Flattery,
    Acting,
    Incitement,
    Intimidation,
    Evasion,
    Charm,
    Brainwashing,
    Slander,
    BlameShifting,
    Silence,
    Tact
}

public enum MethodType
{
    None,
    Combat,
    Might,
    Durability,
    Agility,
    Reflex,
    Enhancement,
    Sensation,
    Perseverance,
    Stealth,
    Detection,
    Dexterity,
    Resourcefulness
}

public enum ResistanceType
{
    None,
    Predation,
    Parasitism,
    Infection,
    Mutation,
    Hallucination,
    Mental,
    Death,
    Space,
    Time,
    Void,
    Divine,
    Knowledge
}