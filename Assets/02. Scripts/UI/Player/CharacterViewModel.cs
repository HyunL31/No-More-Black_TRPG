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
                return CurrentCharacterModel.SelectedMajor.Contains(skillID);

            case SkillCategory.Career:
                return CurrentCharacterModel.SelectedCareer.Contains(skillID);

            case SkillCategory.Nature:
                return CurrentCharacterModel.SelectedNature == skillID;

            case SkillCategory.Wisdom:
                return CurrentCharacterModel.SelectedWisdom.Contains(skillID);

            case SkillCategory.Method:
                return CurrentCharacterModel.SelectedMethod.Contains(skillID);

            case SkillCategory.Resistance:
                return CurrentCharacterModel.SelectedResistance.Contains(skillID);
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

    public void CheckOperationValue()
    {
        if (Stamina <= 0 || Crazy >= 10)
        {
            AddOblivion(1);
            InitOperationValue();

            // TODO : Info UI, 역량 포기 UI
        }

        if (Oblivion == 10)
        {
            UIManager.Inst.OpenUI(UIRoot.Popup, UIType.GameOverUI);
        }
    }

    private void InitOperationValue()
    {
        Stamina = GameDataManager.Inst.GetData<Character>(CurrentCharacter).Stamina;
        Crazy = GameDataManager.Inst.GetData<Character>(CurrentCharacter).Crazy;
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
                AddList(model.SelectedMajor, skillID);
                break;
            
            case "Career":
                AddList(model.SelectedCareer, skillID);
                break;
            
            case "Wisdom":
                AddList(model.SelectedWisdom, skillID);
                break;
            
            case "Method":
                AddList(model.SelectedMethod, skillID);
                break;
            
            case "Resistance":
                AddList(model.SelectedResistance, skillID);
                break;

            case "Nature":
                model.SelectedNature = skillID;
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

    private static void AddList(List<string> list, string value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
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
