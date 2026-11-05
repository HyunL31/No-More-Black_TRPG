using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VirusUI : MonoBehaviour
{
    [SerializeField] private VirusSlot Prefab_Slot;
    [SerializeField] private Transform Tramsform_Parent;
    [SerializeField] private ScrollRect Scroll_Rect;
    [SerializeField] private float Speed_Slot;

    private ObjectPooling<VirusSlot> _virusPool;
    private readonly List<VirusSlot> _activeSlots = new List<VirusSlot>();

    private int _currentAnswer;

    private void Awake()
    {
        _virusPool = new ObjectPooling<VirusSlot>(Prefab_Slot, Tramsform_Parent, 30, 100);
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
        slot.SetText(AddAnswerCount);
    }

    private void ClearVirusSlot()
    {
        foreach (VirusSlot slot in _activeSlots)
        {
            _virusPool.Release(slot);
        }

        _activeSlots.Clear();
    }

    private int AddAnswerCount()
    {
        return ++_currentAnswer;
    }
}
