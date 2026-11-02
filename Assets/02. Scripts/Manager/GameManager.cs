using System;

public class GameManager : SingletonBase<GameManager>
{
    public Action<float> OnChangedBrightness;
}
