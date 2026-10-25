using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Toggle Toggle_Skill;
    [SerializeField] private TextMeshProUGUI Text_SkillName;

    private Action<bool> _onValueChanged;

    public void InitSlot(Action<bool> onValueChanged)
    {
        _onValueChanged = onValueChanged;
        Toggle_Skill.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        _onValueChanged?.Invoke(isOn);
    }

    public void SetSlot(string skillName, bool isActive, bool canInteract)
    {
        Text_SkillName.text = skillName;

        Toggle_Skill.onValueChanged.RemoveListener(OnToggleChanged);
        Toggle_Skill.isOn = isActive;
        Toggle_Skill.onValueChanged.AddListener(OnToggleChanged);

        Toggle_Skill.interactable = !isActive && canInteract;
    }
}