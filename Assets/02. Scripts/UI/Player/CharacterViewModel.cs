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
            if (_playerModel.Characters[i].ID == CurrentCharacter)
            {
                return i;
            }
        }

        return 0;
    }

    public bool CheckSkill(SkillCategory category, string skillID)
    {
        var skill = GameDataManager.Inst.GetData<Skill>(skillID);
        if (skill == null)
        {
            return false;
        }

        switch (category)
        {
            case SkillCategory.Major:
                if (Enum.TryParse<MajorType>(skill.EnName, out var majorType))
                {
                    return CurrentCharacterModel.SelectedMajor.Contains(majorType);
                }
                break;

            case SkillCategory.Career:
                if (Enum.TryParse<CareerType>(skill.EnName, out var careerType))
                {
                    return CurrentCharacterModel.SelectedCareer.Contains(careerType);
                }
                break;

            case SkillCategory.Nature:
                if (Enum.TryParse<NatureType>(skill.EnName, out var natureType))
                {
                    return CurrentCharacterModel.SelectedNature == natureType;
                }
                break;

            case SkillCategory.Wisdom:
                if (Enum.TryParse<WisdomType>(skill.EnName, out var wisdomType))
                {
                    return CurrentCharacterModel.SelectedWisdom.Contains(wisdomType);
                }
                break;

            case SkillCategory.Method:
                if (Enum.TryParse<MethodType>(skill.EnName, out var methodType))
                {
                    return CurrentCharacterModel.SelectedMethod.Contains(methodType);
                }
                break;

            case SkillCategory.Resistance:
                if (Enum.TryParse<ResistanceType>(skill.EnName, out var resistanceType))
                {
                    return CurrentCharacterModel.SelectedResistance.Contains(resistanceType);
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
                AddEnum(model.SelectedMajor, skill.EnName);
                break;
            
            case "Career":
                AddEnum(model.SelectedCareer, skill.EnName);
                break;
            
            case "Wisdom":
                AddEnum(model.SelectedWisdom, skill.EnName);
                break;
            
            case "Method":
                AddEnum(model.SelectedMethod, skill.EnName);
                break;
            
            case "Resistance":
                AddEnum(model.SelectedResistance, skill.EnName);
                break;

            case "Nature":
                if (Enum.TryParse<NatureType>(skill.EnName, true, out var natureParsed))
                {
                    model.SelectedNature = natureParsed;
                }
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
