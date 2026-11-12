using Unity.Cinemachine;
using UnityEngine;

public class GameObjectManager : SingletonBase<GameObjectManager>
{
    [SerializeField] private Camera Camera_Dice;
    [SerializeField] private DiceController Prefab_PlayerDice;
    [SerializeField] private DiceController Prefab_OpponentDice;
    [SerializeField] private GameObject SpaceshipMap;
    [SerializeField] private CinemachineCamera Camera_Cinemachine;
    [SerializeField] private GameObject Player;

    private GameObject _spaceship;
    private DiceController _playerDice;
    private DiceController _opponentDice;

    public void InitMap()
    {
        if (_spaceship == null)
        {
            _spaceship = Instantiate(SpaceshipMap);
            InitPlayer();
        }

        UIManager.Inst.OpenMainUI();
        UIManager.Inst.CloseDialogueUI();
        UIManager.Inst.CloseDialogueLog();
        UIManager.Inst.CloseVisualNovelUI();
        UIManager.Inst.CloseBackground();

        UIManager.Inst.OpenMainUI();
    }

    private void InitPlayer()
    {
        Player = Instantiate(Player);
        Camera_Cinemachine.Target.TrackingTarget = Player.transform;
    }

    public DiceController InitPlayerDice()
    {
        if (_playerDice == null)
        {
            _playerDice = Instantiate(Prefab_PlayerDice);
            return _playerDice;
        }
        else
        {
            _playerDice.gameObject.SetActive(true);
            return _playerDice;
        }
    }

    public DiceController InitOpponentDice()
    {
        if (_opponentDice == null)
        {
            _opponentDice = Instantiate(Prefab_OpponentDice);
            return _opponentDice;
        }
        else
        {
            _opponentDice.gameObject.SetActive(true);
            return _opponentDice;
        }
    }

    public void CloseDice()
    {
        _playerDice.gameObject.SetActive(false);
        _opponentDice.gameObject.SetActive(false);
    }

    public GameObject GetPlayer()
    {
        return Player;
    }

    public Camera GetDiceCamera()
    {
        return Camera_Dice;
    }
}
