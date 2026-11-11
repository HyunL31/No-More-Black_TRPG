using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    public string PlayerName;
    public bool IsNewGame;

    public string SaveChapter;
    public string SaveDate;

    public Dictionary<string, int> Inventory = new Dictionary<string, int>();
    public List<string> CollectedItem = new List<string>();

    public Dictionary<string, List<int>> PersonalEvent = new Dictionary<string, List<int>>();
    public List<CharacterModel> Characters = new List<CharacterModel>();
}