using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_Dialogue;
    [SerializeField] private Image Image_Portrait;

    public async UniTask SetSlot(string characterID, string content)
    {
        Text_Dialogue.text = content;

        Image_Portrait.sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{characterID}");
    }
}
