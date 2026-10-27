using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : SingletonBase<SaveManager>
{
    public PlayerModel PlayerModel { get; private set; }

    public SortedSet<int> SlotIndex { get; private set; } = new SortedSet<int>();
    public int CurrentSlotIndex { get; private set; } = 0;

    private bool _isInit = true;

    private void Start()
    {
        Initialize();

        // 임시
        PlayerModel = RequestLoadData(0);
    }

    public void Initialize()
    {
        if (_isInit == false)
        {
            return;
        }

        SlotIndex.Clear();

        for (int i = 0; i < 10; i++)
        {
            if (HasSaveFile(i) == true)
            {
                SlotIndex.Add(i);
            }
        }

        _isInit = false;
    }

    private string GetPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"NoMoreBlack{slotIndex}.json");
    }

    public void RequestSaveData(PlayerModel data)
    {
        data.SaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(CurrentSlotIndex), json);
    }

    public PlayerModel RequestLoadData(int slotIndex)
    {
        string path = GetPath(slotIndex);
        CurrentSlotIndex = slotIndex;

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

    public int GetEmptyIndex()
    {
        if (SlotIndex.Count < 30)
        {
            for (int i = 0; i < 30; i++)
            {
                if (SlotIndex.Contains(i) == false)
                {
                    return i;
                }
            }
        }

        return 31;
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetPath(slotIndex));
    }
}
