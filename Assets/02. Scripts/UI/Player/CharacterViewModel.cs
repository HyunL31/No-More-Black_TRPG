using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewModel : ViewModelBase
{
    private PlayerModel _playerModel;
    public string CurrentCharacter { get; private set; } = "Player_01";

    public CharacterViewModel()
    {
        _playerModel = SaveManager.Inst.PlayerModel;
    }

    public void SetCurrentCharacter(string currentCharacter)
    {
        foreach (CharacterModel character in _playerModel.Characters)
        {
            if (character.Name == GameDataManager.Inst.GetData<Character>(currentCharacter).Name)
            {
                CurrentCharacter = currentCharacter;

                OnPropertyChanged(nameof(Stamina));
                OnPropertyChanged(nameof(Crazy));
                OnPropertyChanged(nameof(Oblivion));
                OnPropertyChanged(nameof(Ideology));
                break;
            }
        }
    }

    private int GetCurrentIndex()
    {
        for (int i = 0; i < _playerModel.Characters.Count; i++)
        {
            CharacterModel character = _playerModel.Characters[i];

            if (character.Name == GameDataManager.Inst.GetData<Character>(CurrentCharacter).Name)
            {
                return i;
            }
        }

        return 0;
    }

    public bool CheckSkill(SkillCategory category, int index)
    {
        switch (category)
        {
            case SkillCategory.Major:
                if (Enum.IsDefined(typeof(MajorType), index))
                {
                    MajorType type = (MajorType)index;
                    return CurrentCharacterModel.SelectedMajor.Contains(type);
                }
                break;

            case SkillCategory.Career:
                if (Enum.IsDefined(typeof(CareerType), index))
                {
                    CareerType type = (CareerType)index;
                    return CurrentCharacterModel.SelectedCareer.Contains(type);
                }
                break;

            case SkillCategory.Nature:
                if (Enum.IsDefined(typeof(NatureType), index))
                {
                    NatureType type = (NatureType)index;
                    return CurrentCharacterModel.SelectedNature.Contains(type);
                }
                break;

            case SkillCategory.Wisdom:
                if (Enum.IsDefined(typeof(WisdomType), index))
                {
                    WisdomType type = (WisdomType)index;
                    return CurrentCharacterModel.SelectedWisdom.Contains(type);
                }
                break;

            case SkillCategory.Method:
                if (Enum.IsDefined(typeof(MethodType), index))
                {
                    MethodType type = (MethodType)index;
                    return CurrentCharacterModel.SelectedMethod.Contains(type);
                }
                break;

            case SkillCategory.Resistance:
                if (Enum.IsDefined(typeof(ResistanceType), index))
                {
                    ResistanceType type = (ResistanceType)index;
                    return CurrentCharacterModel.SelectedResistance.Contains(type);
                }
                break;
        }
        return false;
    }

    public bool CheckAbandonSkill(string skillID)
    {
        if (CurrentCharacterModel.AbandonedSkillID.Contains(skillID))
        {
            return true;
        }

        return false;
    }

    public void AddStamina(int delta)
    {
        Stamina = Mathf.Clamp(Stamina + delta, 0, 10);
    }

    public void AddCrazy(int delta)
    {
        Crazy = Mathf.Clamp(Crazy + delta, 0, 10);
    }

    public void AddOblivion(int delta)
    {
        Oblivion = Mathf.Clamp(Oblivion + delta, 0, 10);
    }

    public void AddIdeology(int delta)
    {
        Ideology = Mathf.Clamp(Ideology + delta, -3, 3);
    }

    public void GetSkill(string skillID)
    {
        Skill skill = GameDataManager.Inst.GetData<Skill>(skillID);
        if (skill == null)
        {
            return;
        }

        CharacterModel model = CurrentCharacterModel;

        if (!Enum.TryParse<SkillCategory>(skill.Category, out SkillCategory category))
        {
            return;
        }

        if (!model.CategoryUnlock.Contains(category))
        {
            model.CategoryUnlock.Add(category);
        }

        switch (skill.Category)
        {
            case "Major":
                AddEnum(model.SelectedMajor, skill.Name);
                break;
            
            case "Career":
                AddEnum(model.SelectedCareer, skill.Name);
                break;
            
            case "Nature":
                AddEnum(model.SelectedNature, skill.Name);
                break;
            
            case "Wisdom":
                AddEnum(model.SelectedWisdom, skill.Name);
                break;
            
            case "Method":
                AddEnum(model.SelectedMethod, skill.Name);
                break;
            
            case "Resistance":
                AddEnum(model.SelectedResistance, skill.Name);
                break;
        }
    }

    public void AbandonSkill(string skillID)
    {
        if (!CurrentCharacterModel.AbandonedSkillID.Contains(skillID))
        {
            CurrentCharacterModel.AbandonedSkillID.Add(skillID);
        }
    }

    private static void AddEnum<T>(List<T> list, string value) where T : struct
    {
        if (Enum.TryParse<T>(value, out var parsed) && !list.Contains(parsed))
        {
            list.Add(parsed);
        }
    }


    public CharacterModel CurrentCharacterModel
    {
        get
        {
            int idx = GetCurrentIndex();
            return _playerModel.Characters[idx];
        }
    }

    public int Stamina
    {
        get => CurrentCharacterModel.Stamina;
        set
        {
            CharacterModel model = CurrentCharacterModel;
            if (model.Stamina != value)
            {
                model.Stamina = value;
                OnPropertyChanged(nameof(Stamina));
            }
        }
    }

    public int Crazy
    {
        get => CurrentCharacterModel.Crazy;
        set
        {
            CharacterModel model = CurrentCharacterModel;
            if (model.Crazy != value)
            {
                model.Crazy = value;
                OnPropertyChanged(nameof(Crazy));
            }
        }
    }

    public int Oblivion
    {
        get => CurrentCharacterModel.Oblivion;
        set
        {
            int idx = GetCurrentIndex();
            if (_playerModel.Characters[idx].Oblivion != value)
            {
                _playerModel.Characters[idx].Oblivion = value;
                OnPropertyChanged(nameof(Oblivion));
            }
        }
    }

    public int Ideology
    {
        get => CurrentCharacterModel.Ideology;
        set
        {
            int idx = GetCurrentIndex();
            if (_playerModel.Characters[idx].Ideology != value)
            {
                _playerModel.Characters[idx].Ideology = value;
                OnPropertyChanged(nameof(Ideology));
            }
        }
    }
}
