using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    public string PlayerName { get; set; }

    public List<CharacterModel> Characters { get; set; } = new List<CharacterModel>();
}