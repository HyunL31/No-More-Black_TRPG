using Unity.Cinemachine;
using UnityEngine;

public class GameObjectManager : SingletonBase<GameObjectManager>
{
    [SerializeField] private GameObject SpaceshipMap;
    [SerializeField] private GameObject Player;
    [SerializeField] private CinemachineCamera Camera_Cinemachine;

    private GameObject _spaceship;

    public void InitMap()
    {
        if (_spaceship == null)
        {
            _spaceship = Instantiate(SpaceshipMap);
            InitPlayer();
        }

        UIManager.Inst.CloseDialogueUI();
        UIManager.Inst.CloseDialogueLog();
        UIManager.Inst.CloseVisualNovelUI();
        UIManager.Inst.CloseBackground();

        //UIManager.Inst.OpenSpaceshipUI();
    }

    private void InitPlayer()
    {
        Player = Instantiate(Player);
        Camera_Cinemachine.Target.TrackingTarget = Player.transform;
    }
}
