using System.IO;
using UnityEngine;

public class SaveManager : SingletonBase<SaveManager>
{
    public PlayerModel PlayerModel { get; private set; }

    private void Start()
    {
        // 임시
        PlayerModel = RequestLoadData(0);
    }

    private string GetPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"NoMoreBlack{slotIndex}.json");
    }

    public void RequestSaveData(int slotIndex, PlayerModel data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slotIndex), json);
    }

    public PlayerModel RequestLoadData(int slotIndex)
    {
        string path = GetPath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerModel data = JsonUtility.FromJson<PlayerModel>(json);
            this.PlayerModel = data;
            return data;
        }
        else
        {
            var playerData = GetDefaultData();
            this.PlayerModel = playerData;
            return playerData;
        }
    }

    public PlayerModel GetDefaultData()
    {
        PlayerModel newPlayerData = new PlayerModel();
        newPlayerData.PlayerName = "";
        newPlayerData.IsNewGame = true;

        string[] characterKeys = { "Player_01", "Elysia_01", "July_01", "Taehyun_01", "Labresa_01" };

        foreach (var key in characterKeys)
        {
            Character rawData = GameDataManager.Inst.GetData<Character>(key);

            if (rawData != null)
            {
                bool isPlayer = (key == "Player_01");
                newPlayerData.Characters.Add(new CharacterModel(rawData, isPlayer));
            }
        }

        return newPlayerData;
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetPath(slotIndex));
    }
}
