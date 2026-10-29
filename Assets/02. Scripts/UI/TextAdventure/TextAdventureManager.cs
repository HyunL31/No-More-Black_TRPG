using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    None,
    Stamina,
    Crazy,
    Oblivion,
    Skill
}

public class TextAdventureManager : SingletonBase<TextAdventureManager>
{
    public string CurrentID { get; private set; } = "Dialogue_01";

    public List<KeyValuePair<string, string>> DialogueLogs { get; private set; } = new List<KeyValuePair<string, string>>();

    public Action<string, string> OnAddLog;
    public Action<string> OnChangeBaseUI;
    public Action OnReturnDialogue;

    private CharacterViewModel _characterVM;

    protected override void Awake()
    {
        base.Awake();
        OnAddLog = AddDialogueLog;
    }

    private void AddDialogueLog(string characterID, string content)
    {
        DialogueLogs.Add(new KeyValuePair<string, string>(characterID, content));
    }

    public void ApplyResult(string resultID)
    {
        if (string.IsNullOrEmpty(resultID) == true)
        {
            return;
        }

        Result result = GameDataManager.Inst.GetData<Result>(resultID);

        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        }

        int value = Mathf.RoundToInt(result.Value);

        switch (result.Type)
        {
            case "Stamina":
                int a = _characterVM.Stamina;
                _characterVM.AddStamina(value);
                Debug.Log($"[Result] {resultID} | Stamina: {a} → {_characterVM.Stamina} (변화량 {value})");
                break;

            case "Crazy":
                int b = _characterVM.Crazy;
                _characterVM.AddCrazy(value);
                Debug.Log($"[Result] {resultID} | Crazy: {b} → {_characterVM.Crazy} (변화량 {value})");
                break;

            case "Oblivion":
                int c = _characterVM.Oblivion;
                _characterVM.AddOblivion(value);
                Debug.Log($"[Result] {resultID} | Oblivion: {c} → {_characterVM.Oblivion} (변화량 {value})");
                break;

            case "Ideology":
                int d = _characterVM.Ideology;
                _characterVM.AddIdeology(value);
                Debug.Log($"[Result] {resultID} | Ideology: {d} → {_characterVM.Ideology} (변화량 {value})");
                break;

            case "Skill":
                if (result.IsGet == true)
                {
                    _characterVM.GetSkill(result.SkillID);
                }
                else
                {
                    _characterVM.AbandonSkill(result.SkillID);
                }

                Debug.Log($"[Result] {resultID} | Skill {(result.IsGet ? "획득" : "포기")}: {result.SkillID} | 캐릭터: {_characterVM.CurrentCharacter}");
                break;

            case "Relationship":

                Enum.TryParse<RelationType>(result.RelationType, out RelationType type);

                if (result.IsGet == true)
                {
                    _characterVM.CurrentCharacterModel.SetRelation(result.SkillID, type);
                }
                else
                {
                    _characterVM.CurrentCharacterModel.RemoveRelation(result.SkillID, type);
                }

                Debug.Log($"[Result] {resultID} | Relationship {(result.IsGet ? "획득" : "포기")}: {result.SkillID} | 인연: {result.RelationType}");
                break;

            default:
                break;
        }
    }

    public bool CheckCondition(string[] conditions, string[] operations)
    {
        if (conditions.Length == 0)
        {
            return true;
        }

        bool condition = EvaluateCondition(conditions[0]);

        for (int i = 1; i < conditions.Length; i++)
        {
            if (operations[i - 1] == "OR")
            {
                if (condition == true)
                {
                    return true;
                }

                condition = EvaluateCondition(conditions[i]);
            }
            else
            {
                if (condition == true)
                {
                    EvaluateCondition(conditions[i]);
                }
            }
        }

        return condition;
    }

    private bool EvaluateCondition(string conditionID)
    {
        var data = GameDataManager.Inst.GetData<Condition>(conditionID);

        switch (data.ConditionType)
        {
            case "Skill":
                string skillType = GameDataManager.Inst.GetData<Skill>(data.TargetID).Category;
                Enum.TryParse(skillType, out SkillCategory category);

                bool hasSkill = _characterVM.CheckSkill(category, data.TargetID);
                return data.Operation == "!=" ? !hasSkill : hasSkill;

            case "Relationship":
                Enum.TryParse(data.RelationType, out RelationType type);
                bool isRelation = _characterVM.CurrentCharacterModel.GetRelation(data.TargetID, type) != null;
                return data.Operation == "!=" ? !isRelation : isRelation;

            case "Stamina":
                float stamina = _characterVM.Stamina;
                return GameUtil.Compare(stamina, data.Value, data.Operation);

            case "Crazy":
                float crazy = _characterVM.Crazy;
                return GameUtil.Compare(crazy, data.Value, data.Operation);

            case "Oblivion":
                float oblivion = _characterVM.Oblivion;
                return GameUtil.Compare(oblivion, data.Value, data.Operation);

            case "Ideology":
                float ideology = _characterVM.Ideology;
                return GameUtil.Compare(ideology, data.Value, data.Operation);

            default:
                return false;
        }
    }

    public void SetCurrentID(string id)
    {
        CurrentID = id;
    }
}
