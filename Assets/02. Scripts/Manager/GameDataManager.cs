using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : SingletonBase<GameDataManager>
{
    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> datas;
    }

    private Dictionary<Type, object> _allDataDict = new Dictionary<Type, object>();

    protected override void Awake()
    {
        base.Awake();
        LoadAllData();
    }

    public void LoadAllData()
    {
        LoadData<Character>("Character");
        LoadData<Skill>("Skill");
        LoadData<Dialogue>("Dialogue");
        LoadData<Choice>("Choice");
        LoadData<Polygraph>("Polygraph");
        LoadData<Inspector>("Inspector");
        LoadData<Result>("Result");
    }

    private void LoadData<T>(string table) where T : GameDataBase
    {
        string resourcePath = $"JsonOutput/{table}";
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        Dictionary<string, T> dataDict = new Dictionary<string, T>();

        if (textAsset != null)
        {
            try
            {
                string jsonString = textAsset.text;
                string wrappedJson = "{\"datas\":" + jsonString + "}";
                SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

                if (wrapper != null && wrapper.datas != null)
                {
                    foreach (T data in wrapper.datas)
                    {
                        if (!dataDict.ContainsKey(data.ID))
                        {
                            dataDict.Add(data.ID, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
            }
        }

        if (!_allDataDict.ContainsKey(typeof(T)))
        {
            _allDataDict.Add(typeof(T), dataDict);
        }
    }

    public List<T> GetAllData<T>() where T : GameDataBase
    {
        if (_allDataDict.TryGetValue(typeof(T), out object dictObj) == false)
        {
            return new List<T>();
        }

        Dictionary<string, T> dict = dictObj as Dictionary<string, T>;

        if (dict == null)
        {
            return new List<T>();
        }

        return new List<T>(dict.Values);
    }

    public T GetData<T>(string id) where T : GameDataBase
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        object dictObj;

        if (_allDataDict.TryGetValue(typeof(T), out dictObj))
        {
            Dictionary<string, T> dict = dictObj as Dictionary<string, T>;

            if (dict != null)
            {
                T item;

                if (dict.TryGetValue(id, out item))
                {
                    return item;
                }
            }
        }

        return null;
    }

    public List<T> GetDataList<T>() where T : GameDataBase
    {
        if (_allDataDict.TryGetValue(typeof(T), out object dictObj))
        {
            if (dictObj is Dictionary<string, T> dict)
            {
                return new List<T>(dict.Values);
            }
        }

        return new List<T>();
    }
}