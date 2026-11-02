using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : SingletonBase<SaveManager>
{
    public PlayerModel PlayerModel { get; private set; }

    public SortedSet<string> SlotDate { get; private set; } = new SortedSet<string>();
    public string CurrentSlotDate { get; private set; } = "";

    private bool _isInit = true;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_isInit == false)
        {
            return;
        }

        SlotDate.Clear();

        string folderPath = Application.persistentDataPath;

        if (Directory.Exists(folderPath) == true)
        {
            string[] files = Directory.GetFiles(folderPath, "NoMoreBlack*.json");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string dateStr = fileName.Replace("NoMoreBlack", "");

                SlotDate.Add(dateStr);
            }
        }

        _isInit = false;
    }

    private string GetPath(string slotDate)
    {
        return Path.Combine(Application.persistentDataPath, $"NoMoreBlack{slotDate}.json");
    }

    public void RequestSaveData(PlayerModel data)
    {
        string fileDate = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        data.SaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(fileDate), json);
    }

    public PlayerModel RequestLoadData(string slotDate)
    {
        string path = GetPath(slotDate);
        CurrentSlotDate = slotDate;

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

    public bool HasSaveFile(string slotDate)
    {
        return File.Exists(GetPath(slotDate));
    }
}