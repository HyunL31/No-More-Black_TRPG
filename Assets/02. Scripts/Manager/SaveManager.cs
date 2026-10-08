using System.IO;
using UnityEngine;

public class SaveManager : SingletonBase<SaveManager>
{
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

            return data;
        }
        else
        {
            var playerData = GetDefaultData();
            return playerData;
        }
    }

    public PlayerModel GetDefaultData()
    {
        PlayerModel newPlayerData = new PlayerModel();
        //newPlayerData.PlayerName = "";

        return newPlayerData;
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetPath(slotIndex));
    }
}
