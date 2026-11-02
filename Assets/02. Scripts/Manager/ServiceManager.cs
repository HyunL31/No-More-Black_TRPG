using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    private CharacterViewModel _characterViewModel;

    public CharacterViewModel GetCharacterViewModel()
    {
        if (_characterViewModel == null)
        {
            _characterViewModel = new CharacterViewModel();
        }

        return _characterViewModel;
    }
}
