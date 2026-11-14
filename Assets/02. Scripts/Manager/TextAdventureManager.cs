using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    None,
    Stamina,
    Crazy,
    Oblivion,
    Skill,
    Impart
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

    public bool ApplyResult(string resultID)
    {
        if (string.IsNullOrEmpty(resultID) == true)
        {
            return false;
        }

        Result result = GameDataManager.Inst.GetData<Result>(resultID);

        if (_characterVM == null)
        {
            _characterVM = ServiceManager.Inst.GetCharacterViewModel();
        }

        _characterVM.SetCurrentCharacter(result.ApplyTarget);

        int value = Mathf.RoundToInt(result.Value);

        switch (result.Type)
        {
            case "Stamina":
                int a = _characterVM.Stamina;
                _characterVM.AddStamina(value);
                _characterVM.CheckOperationValue();

                Debug.Log($"[Result] {resultID} | Stamina: {a} → {_characterVM.Stamina} (변화량 {value})");
                break;

            case "Crazy":
                int b = _characterVM.Crazy;
                _characterVM.AddCrazy(value);
                _characterVM.CheckOperationValue();
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
                {
                    string skill = GameDataManager.Inst.GetData<Skill>(result.SkillID).Name;
                    string characterName = GameDataManager.Inst.GetData<Character>(_characterVM.CurrentCharacter).Name;

                    if (result.IsGet == true)
                    {
                        UIManager.Inst.OpenInfoUI(GameUtil.GetPopupMessage("{0}이(가) 역량 {1}을(를) 획득했습니다.", characterName, skill), () => _characterVM.GetSkill(result.SkillID));
                    }
                    else
                    {
                        UIManager.Inst.OpenInfoUI(GameUtil.GetPopupMessage("{0}이(가) 역량 {1}을(를) 잃었습니다.", characterName, skill), () => _characterVM.AbandonSkill(result.SkillID));
                    }

                    return true;
                }

            case "Impart":
                {
                    string npc = GameDataManager.Inst.GetData<Character>(result.TargetID).Name;
                    string skill = GameDataManager.Inst.GetData<Skill>(result.SkillID).Name;

                    UIManager.Inst.OpenInfoUI(GameUtil.GetPopupMessage("{0}의 역량: {1}을(를) 획득하시겠습니까?", npc, skill), () => _characterVM.GetSkill(result.SkillID), false);

                    return true;
                }

            case "Relationship":
                {
                    string npc = GameDataManager.Inst.GetData<Character>(result.TargetID).Name;

                    Enum.TryParse<RelationType>(result.RelationType, out RelationType type);
                    string relation = GameUtil.GetRelationText(type);

                    if (result.IsGet == true)
                    {
                        UIManager.Inst.OpenInfoUI(GameUtil.GetPopupMessage("{0}와 {1}의 인연을 맺었습니다.", npc, relation), () => _characterVM.CurrentCharacterModel.SetRelation(result.TargetID, type));
                    }
                    else
                    {
                        UIManager.Inst.OpenInfoUI(GameUtil.GetPopupMessage("{0}와 {1}의 인연을 잃었습니다.", npc, relation), () => _characterVM.CurrentCharacterModel.RemoveRelation(result.TargetID, type));
                    }

                    return true;
                }
        }

        return false;
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
