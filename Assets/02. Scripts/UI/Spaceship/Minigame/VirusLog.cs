using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VirusLog : MonoBehaviour
{
    [SerializeField] private VirusUI VirusUI;

    [SerializeField] private VirusSlot Prefab_Slot;
    [SerializeField] private Transform Transform_Parent;
    [SerializeField] private ScrollRect Scroll_Rect;
    [SerializeField] private float Speed_Slot;

    private ObjectPooling<VirusSlot> _virusPool;
    private readonly List<VirusSlot> _activeSlots = new List<VirusSlot>();


    private void Awake()
    {
        _virusPool = new ObjectPooling<VirusSlot>(Prefab_Slot, Transform_Parent, 30, 100);
    }

    private void OnEnable()
    {
        ClearVirusSlot();

        PritLog().Forget();
    }

    private async UniTask PritLog()
    {
        while (true)
        {
            ShowLog();

            Canvas.ForceUpdateCanvases();
            Scroll_Rect.verticalNormalizedPosition = 0f;

            await UniTask.Delay(TimeSpan.FromSeconds(Speed_Slot));
        }
    }

    private void ShowLog()
    {
        int random = Random.Range(1, 31);

        VirusSlot slot = _virusPool.Get();
        slot.SetVirusID($"Virus_{random}");
        slot.SetText(VirusUI.AddAnswerCount);
    }

    private void ClearVirusSlot()
    {
        foreach (VirusSlot slot in _activeSlots)
        {
            _virusPool.Release(slot);
        }

        _activeSlots.Clear();
    }
}
