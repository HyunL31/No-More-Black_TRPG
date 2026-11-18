using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class DiceEditSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_EditInfo;
    [SerializeField] private GameObject Effect_Hit;

    private string _editID;
    private Vector3 _baseScale;
    private bool _initialized;
    private Sequence _hit;

    public string EditID => _editID;

    private void Init()
    {
        if (_initialized == true)
        {
            return;
        }

        _baseScale = transform.localScale;
        _initialized = true;
    }

    private void OnDisable()
    {
        _hit?.Kill();
        _hit = null;
    }

    public void SetEditInfo(string editID, bool isOpponent)
    {
        Init();
        ResetSlot();

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

    public void PlayHit()
    {
        Init();
        _hit?.Kill();

        Effect_Hit.SetActive(true);

        _hit = DOTween.Sequence().SetLink(gameObject);
        _hit.Append(transform.DOScale(_baseScale * 1.4f, 0.08f).SetEase(Ease.OutQuad));

        _hit.Append(transform.DOScale(Vector3.zero, 0.08f).SetEase(Ease.OutQuad));
    }

    public void ResetSlot()
    {
        Init();
        _hit?.Kill();
        _hit = null;

        Effect_Hit.SetActive(false);
        transform.localScale = _baseScale;
    }
}
