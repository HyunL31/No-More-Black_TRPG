using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_Dialogue;
    [SerializeField] private Image Image_Portrait;

    public async UniTask SetSlot(string contentID)
    {
        if (contentID.Contains("Dialogue"))
        {
            var data = GameDataManager.Inst.GetData<Dialogue>(contentID);

            Text_Dialogue.text = data.Content;
            Image_Portrait.sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{data.Speaker}");
        }
    }
}
