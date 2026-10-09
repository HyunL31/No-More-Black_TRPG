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
