using System;
using UnityEngine;

public static class GameUtil
{
    public static void ApplyResult(string resultID)
    {
        if (string.IsNullOrEmpty(resultID) == true)
        {
            return;
        }

        Result result = GameDataManager.Inst.GetData<Result>(resultID);

        CharacterViewModel characterVM = ServiceManager.Inst.GetCharacterViewModel();
        int value = Mathf.RoundToInt(result.Value);

        switch (result.Type)
        {
            case "Stamina":
                int a = characterVM.Stamina;
                characterVM.AddStamina(value);
                Debug.Log($"[Result] {resultID} | Stamina: {a} → {characterVM.Stamina} (변화량 {value})");
                break;

            case "Crazy":
                int b = characterVM.Crazy;
                characterVM.AddCrazy(value);
                Debug.Log($"[Result] {resultID} | Crazy: {b} → {characterVM.Crazy} (변화량 {value})");
                break;

            case "Oblivion":
                int c = characterVM.Oblivion;
                characterVM.AddOblivion(value);
                Debug.Log($"[Result] {resultID} | Oblivion: {c} → {characterVM.Oblivion} (변화량 {value})");
                break;

            case "Ideology":
                int d = characterVM.Ideology;
                characterVM.AddIdeology(value);
                Debug.Log($"[Result] {resultID} | Ideology: {d} → {characterVM.Ideology} (변화량 {value})");
                break;

            case "Skill":
                if (result.IsGet == true)
                {
                    characterVM.GetSkill(result.SkillID);
                }
                else
                {
                    characterVM.AbandonSkill(result.SkillID);
                }

                Debug.Log($"[Result] {resultID} | Skill {(result.IsGet ? "획득" : "포기")}: {result.SkillID} | 캐릭터: {characterVM.CurrentCharacter}");
                break;

            case "Relationship":

                Enum.TryParse<RelationType>(result.RelationType, out RelationType type);

                if (result.IsGet == true)
                {
                    characterVM.CurrentCharacterModel.SetRelation(result.SkillID, type);
                }
                else
                {
                    characterVM.CurrentCharacterModel.RemoveRelation(result.SkillID, type);
                }

                Debug.Log($"[Result] {resultID} | Relationship {(result.IsGet ? "획득" : "포기")}: {result.SkillID} | 인연: {result.RelationType}");
                break;

            default:
                break;
        }
    }
}