using System;

public class GameManager : SingletonBase<GameManager>
{
    private CharacterViewModel _characterVM;

    public int Day => SaveManager.Inst.PlayerModel.Day;

    public Action<float> OnChangedBrightness;

    private void AddDay()
    {
        SaveManager.Inst.PlayerModel.Day++;
        
        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        }

        _characterVM.Oblivion--;
    }
}
