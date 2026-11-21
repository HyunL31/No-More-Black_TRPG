using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLog : ViewBase
{
    [SerializeField] private PanelSlideController SlideController;
    [SerializeField] private Button Button_Close;
    [SerializeField] private Transform Transform_LogParent;
    [SerializeField] private LogSlot Prefab_LeftLogSlot;
    [SerializeField] private LogSlot Prefab_RightLogSlot;
    [SerializeField] private ScrollRect ScrollRect_Log;

    private ObjectPooling<LogSlot> _leftPool;
    private ObjectPooling<LogSlot> _rightPool;

    private readonly List<LogSlot> _activeSlots = new List<LogSlot>();

    private void Awake()
    {
        Button_Close.onClick.AddListener(OnClickClose);

        _leftPool = new ObjectPooling<LogSlot>(Prefab_LeftLogSlot, Transform_LogParent, 10, 100);
        _rightPool = new ObjectPooling<LogSlot>(Prefab_RightLogSlot, Transform_LogParent, 10, 100);
    }

    private void OnEnable()
    {
        InitSlot().Forget();
    }

    private void OnDisable()
    {
        ClearSlot();
    }

    private void OnClickClose()
    {
        SlideController.SlideOut(Vector2.up, UIManager.Inst.CloseDialogueLog);
    }

    private async UniTaskVoid InitSlot()
    {
        ClearSlot();

        var logs = TextAdventureManager.Inst.DialogueLogs;

        for (int i = 0; i < logs.Count; i++)
        {
            bool isPlayer = logs[i].Key == "Player_01";

            LogSlot slot = isPlayer ? _rightPool.Get() : _leftPool.Get();

            _activeSlots.Add(slot);

            slot.SetSlot(logs[i].Key, logs[i].Value).Forget();
        }

        await UniTask.NextFrame();
        ScrollRect_Log.verticalNormalizedPosition = 0f;
    }

    private void ClearSlot()
    {
        foreach (LogSlot slot in _activeSlots)
        {
            if (slot.name.Contains(Prefab_LeftLogSlot.name))
            {
                _leftPool.Release(slot);
            }
            else
            {
                _rightPool.Release(slot);
            }
        }

        _activeSlots.Clear();
    }
}
