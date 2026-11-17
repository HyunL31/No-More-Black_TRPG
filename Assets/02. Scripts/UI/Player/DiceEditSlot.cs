using System;
using TMPro;
using UnityEngine;

public class DiceEditSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_EditInfo;

    private string _editID;

    public void SetEditInfo(string editID, bool isOpponent)
    {
        _editID = editID;

        var data = GameDataManager.Inst.GetData<DiceEdit>(_editID);

        string edit = "";

        switch (data.EditType)
        {
            case "Skill":
                edit = GameDataManager.Inst.GetData<Skill>(data.EditTarget).Name;
                break;

            case "Relation":
                Enum.TryParse(data.RelationType, out RelationType type);
                edit = GameUtil.GetRelationText(type);
                break;

            case "Ideology":
                int.TryParse(data.EditTarget, out int ideology);
                edit = GameUtil.GetIdeologyName(ideology);
                break;
        }

        string value = Mathf.Abs(data.EditValue).ToString();

        if (isOpponent == true)
        {
            value = new string('■', value.Length);
            edit = new string('■', edit.Length);
        }

        string sign = data.EditValue > 0 ? "+" : "-";

        Text_EditInfo.text = $"{sign}{value} {edit}";
    }
}
