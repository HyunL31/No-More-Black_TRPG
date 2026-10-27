using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private SaveSlot Prefab_SaveSlot;
    [SerializeField] private Transform Transform_SlotParent;

    private ObjectPooling<SaveSlot> _savePool;
    private readonly List<SaveSlot> _activeSlots = new List<SaveSlot>();

    private void Awake()
    {
        _savePool = new ObjectPooling<SaveSlot>(Prefab_SaveSlot, Transform_SlotParent, 3, 10);
    }

    private void OnEnable()
    {
        InitSlot();
    }

    private void InitSlot()
    {
        ClearSaveSlot();

        if (SaveManager.Inst.SlotIndex.Count == 0)
        {
            // 인포 텍스트 처리
            return;
        }

        foreach (int index in SaveManager.Inst.SlotIndex)
        {
            PlayerModel player = SaveManager.Inst.RequestLoadData(index);

            SaveSlot saveSlot = _savePool.Get();
            _activeSlots.Add(saveSlot);

            saveSlot.SetSaveSlot(index, player).Forget();
        }
    }

    private void ClearSaveSlot()
    {
        foreach (SaveSlot slot in _activeSlots)
        {
            _savePool.Release(slot);
        }

        _activeSlots.Clear();
    }
}
