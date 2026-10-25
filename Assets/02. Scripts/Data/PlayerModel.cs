using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    public string PlayerName;
    public bool IsNewGame;

    public List<CharacterModel> Characters = new List<CharacterModel>();
}