using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygraphUI : ViewBase
{
    [SerializeField] private Button Button_Hint;
    [SerializeField] private List<PolyButton> Button_Polys;

    private string _polyID = "";
    private Image _polyAnswer;
    private string _polyCharacterID = "";

    private void Awake()
    {
        Button_Hint.onClick.AddListener(() => OnClickHint().Forget());
    }

    private void OnEnable()
    {
        _polyID = TextAdventureManager.Inst.CurrentID;

        InitPolyButton();
    }

    private async UniTask OnClickHint()
    {
        _polyAnswer.sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{_polyCharacterID}_SD");

        // TODO: 수치 조정 (필요 시)
    }

    private void InitPolyButton()
    {
        int index = 0;
        var data = GameDataManager.Inst.GetDataList<Polygraph>();

        foreach (Polygraph poly in data)
        {
            if (poly.ID.Contains(_polyID))
            {
                if (poly.Answer == true)
                {
                    _polyAnswer = Button_Polys[index].GetCulpritImage();
                    _polyCharacterID = poly.CharacterID;
                }

                Button_Polys[index].InitButton(poly.ID);
                Button_Polys[index].InitState((polyButton) => ClearButtonSelect(polyButton));

                index++;
            }
        }
    }

    private void ClearButtonSelect(PolyButton polyButton)
    {
        foreach (PolyButton button in Button_Polys)
        {
            if (button != polyButton)
            {
                button.Deselect();
            }
        }
    }
}
