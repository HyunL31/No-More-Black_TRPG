using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Toggle Toggle_Skill;
    [SerializeField] private TextMeshProUGUI Text_SkillName;

    public void SetSlot(string skillName, bool isActive)
    {
        Text_SkillName.text = skillName;
        Toggle_Skill.isOn = isActive;
    }
}