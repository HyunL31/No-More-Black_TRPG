using Cysharp.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private Image Image_Thumbnail;
    [SerializeField] private TextMeshProUGUI Text_Date;
    [SerializeField] private Button Button_Select;
    [SerializeField] private Button Button_Delete;

    private string _saveDate;

    private void Awake()
    {
        Button_Select.onClick.AddListener(OnClickSelect);
        Button_Delete.onClick.AddListener(OnClickDiscard);
    }

    private void OnClickSelect()
    {

    }

    private void OnClickDiscard()
    {
        string path = Path.Combine(Application.persistentDataPath, $"NoMoreBlack{_saveDate}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        SaveManager.Inst.SlotDate.Remove(_saveDate);

        Destroy(this.gameObject);
    }

    public async UniTask SetSaveSlot(string date, PlayerModel player)
    {
        _saveDate = date;

        //Image_Thumbnail.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Thumbnail/{player.SaveChapter}");
        Text_Date.text = player.SaveDate;
    }
}
