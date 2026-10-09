using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public CharacterViewModel CharacterViewModel;

    public CharacterViewModel GetCharacterViewModel()
    {
        if (CharacterViewModel == null)
        {
            CharacterViewModel = new CharacterViewModel();
        }

        return CharacterViewModel;
    }
}
