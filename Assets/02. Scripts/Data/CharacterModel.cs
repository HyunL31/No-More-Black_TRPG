using System;
using System.Collections.Generic;

public class CharacterModel
{
    public bool IsPlayer { get; set; }

    // 기본 정보
    public string Name { get; set; }
    public string RegisterCode { get; set; }
    public string Job { get; set; }
    public bool IsGirl { get; set; }
    public int Age { get; set; }
    public float Height { get; set; }

    public string Command { get; set; }
    public string Justice { get; set; }

    public string Major { get; set; }
    public string Organization { get; set; }

    // 운영
    public int Stamina { get; set; }
    public int Crazy { get; set; }
    public int Oblivion { get; set; }
    public int Ideology { get; set; }

    // 인연
    public CharacterModel[] relations { get; set; } = new CharacterModel[6];

    // 역량
    public HashSet<MajorType> SelectedMajors { get; set; } = new HashSet<MajorType>();
    public HashSet<CareerType> SelectedCareers { get; set; } = new HashSet<CareerType>();
    public HashSet<NatureType> SelectedNatures { get; set; } = new HashSet<NatureType>();
    public HashSet<WisdomType> SelectedWisdom { get; set; } = new HashSet<WisdomType>();
    public HashSet<MethodType> SelectedMethod { get; set; } = new HashSet<MethodType>();
    public HashSet<ResistanceType> SelectedResistance { get; set; } = new HashSet<ResistanceType>();

    public CharacterModel() { }
    public CharacterModel(Character character, bool isPlayer = false)
    {
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

        if (!string.IsNullOrEmpty(character.MajorCategory))
        {
            if (Enum.TryParse<MajorType>(character.MajorCategory, out var parsedType))
            {
                SelectedMajors.Add(parsedType);
            }
        }

        if (!string.IsNullOrEmpty(character.Career))
        {
            if (Enum.TryParse<CareerType>(character.Career, out var parsedType))
            {
                SelectedCareers.Add(parsedType);
            }
        }

        if (!string.IsNullOrEmpty(character.Nature))
        {
            if (Enum.TryParse<NatureType>(character.Nature, out var parsedType))
            {
                SelectedNatures.Add(parsedType);
            }
        }

        if (!string.IsNullOrEmpty(character.Wisdom))
        {
            if (Enum.TryParse<WisdomType>(character.Wisdom, out var parsedType))
            {
                SelectedWisdom.Add(parsedType);
            }
        }

        if (!string.IsNullOrEmpty(character.Method))
        {
            if (Enum.TryParse<MethodType>(character.Method, out var parsedType))
            {
                SelectedMethod.Add(parsedType);
            }
        }

        if (!string.IsNullOrEmpty(character.Resistance))
        {
            if (Enum.TryParse<ResistanceType>(character.Resistance, out var parsedType))
            {
                SelectedResistance.Add(parsedType);
            }
        }
    }
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