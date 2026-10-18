using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualNovelUI : ViewBase
{
    [SerializeField] private Image Image_Background;

    [Header("캐릭터 슬롯")]
    [SerializeField] private Transform Transform_Left;
    [SerializeField] private Transform Transform_LeftMiddle;
    [SerializeField] private Transform Transform_Middle;
    [SerializeField] private Transform Transform_RightMiddle;
    [SerializeField] private Transform Transform_Right;

    private Dictionary<string, CharacterState> _characters = new Dictionary<string, CharacterState>();

    private void Awake()
    {
        TextAdventureManager.Inst.OnChangeBaseUI += (id) => ChangeBackground(id).Forget();
        TextAdventureManager.Inst.OnChangeBaseUI += (id) => ApplyCharacter(id);
    }

    private async UniTask ChangeBackground(string id)
    {
        string background = GameDataManager.Inst.GetData<Dialogue>(id).Background;
        Image_Background.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Background/{background}");
    }

    private void ApplyCharacter(string id)
    {
        ClearCharacter();

        var data = GameDataManager.Inst.GetData<Dialogue>(id);

        string[] characters = data.Character;
        string[] slots = data.Slot;
        string[] emotions = data.Emotion;
        string[] animations = data.Animation;

        for (int i = 0; i < characters.Length; i++)
        {
            bool isSpeaker = i == 0 ? true : false;

            ChangeCharacter(characters[i], slots[i], animations[i], emotions[i], isSpeaker).Forget();
        }
    }

    private async UniTask ChangeCharacter(string characterID, string slot, string animation, string skin, bool isSpeaker)
    {
        GameObject prefab = null;

        if (_characters.ContainsKey(characterID) == false)
        {
            prefab = await ResourceManager.Inst.InstantiateAsync($"Spine/{characterID}", GetTransform(slot));
            CharacterState characterState = prefab.GetComponent<CharacterState>();

            _characters.Add(characterID, characterState);
        }
        else
        {
            prefab = _characters[characterID].gameObject;
            prefab.SetActive(true);
            prefab.transform.SetParent(GetTransform(slot));
        }

        _characters[characterID].ApplyEmotion(animation, skin);

        if (isSpeaker == true)
        {
            _characters[characterID].SetWhite();
        }
        else
        {
            _characters[characterID].SetDark();
        }
    }

    private void ClearCharacter()
    {
        foreach (CharacterState state in _characters.Values)
        {
            state.gameObject.SetActive(false);
        }
    }

    private Transform GetTransform(string slot)
    {
        switch (slot)
        {
            case "L":
                return Transform_Left;

            case "LM":
                return Transform_LeftMiddle;

            case "M":
                return Transform_Middle;

            case "RM":
                return Transform_RightMiddle;

            case "R":
                return Transform_Right;
        }

        return transform;
    }
}
