using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class CharacterViewModel : ViewModelBase
{
    private PlayerModel _playerModel;
    public string CurrentCharacter { get; private set; } = "Player_01";

    public CharacterViewModel()
    {
        _playerModel = SaveManager.Inst.RequestLoadData(0);
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
