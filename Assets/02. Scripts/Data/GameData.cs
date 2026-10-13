using System.Collections.Generic;

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
    public List<string> Hostile;
    public List<string> Friction;
    public List<string> Neutral;
    public List<string> Friendly;
    public List<string> Friendship;
    public string Pact;
    public string MajorCategory;
    public string Career;
    public string Nature;
    public string Wisdom;
    public string Method;
    public string Resistance;

    public List<Relation> GetRelations()
    {
        List<Relation> result = new List<Relation>();

        Add(result, Hostile, RelationType.Hostile);
        Add(result, Friction, RelationType.Friction);
        Add(result, Neutral, RelationType.Neutral);
        Add(result, Friendly, RelationType.Friendly);
        Add(result, Friendship, RelationType.Friendship);

        if (!string.IsNullOrEmpty(Pact) && Pact != ID)
        {
            result.Add(new Relation(Pact, RelationType.Pact));
        }

        return result;
    }

    private void Add(List<Relation> result, List<string> ids, RelationType type)
    {
        if (ids == null)
        {
            return;
        }

        foreach (string id in ids)
        {
            if (!string.IsNullOrEmpty(id) && id != ID)
            {
                result.Add(new Relation(id, type));
            }
        }
    }

    [System.Serializable]
    public class Skill : GameDataBase
    {
        public string Name;
        public string Category;
        public bool CanGet;
        public string Description;
    }
}