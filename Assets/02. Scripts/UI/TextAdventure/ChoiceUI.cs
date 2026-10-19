using System.Collections.Generic;
using UnityEngine;

public class ChoiceUI : ViewBase
{
    [SerializeField] private Transform Transform_ChoiceParent;
    [SerializeField] private ChoiceSlot Prefab_Slot;

    private ObjectPooling<ChoiceSlot> _choicePool;
    private readonly List<ChoiceSlot> _activeSlots = new List<ChoiceSlot>();

    private string _choiceID = "";

    private void Awake()
    {
        _choicePool = new ObjectPooling<ChoiceSlot>(Prefab_Slot, Transform_ChoiceParent, 3, 10);

        _choiceID = TextAdventureManager.Inst.CurrentID;
    }

    private void OnEnable()
    {
        InitSlot();
    }

    private void InitSlot()
    {
        ClearSlot();

        var data = GameDataManager.Inst.GetDataList<Choice>();

        foreach (Choice slot in data)
        {
            if (slot.ID.Contains(_choiceID))
            {
                ChoiceSlot choiceSlot = _choicePool.Get();
                _activeSlots.Add(choiceSlot);

                string content = GameDataManager.Inst.GetData<Choice>(slot.ID).Content;

                choiceSlot.SetSlot(content);
                choiceSlot.SetReturnID(slot.NextID);
            }
        }
    }

    private void ClearSlot()
    {
        foreach (ChoiceSlot slot in _activeSlots)
        {
            _choicePool.Release(slot);
        }

        _activeSlots.Clear();
    }
}
