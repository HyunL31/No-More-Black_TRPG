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

    private int _saveIndex;

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
        string path = Path.Combine(Application.persistentDataPath, $"NoMoreBlack{_saveIndex}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        SaveManager.Inst.SlotIndex.Remove(_saveIndex);

        Destroy(this.gameObject);
    }

    public async UniTask SetSaveSlot(int index, PlayerModel player)
    {
        _saveIndex = index;

        //Image_Thumbnail.sprite = await ResourceManager.Inst.LoadSprite($"Sprite/Thumbnail/{player.SaveChapter}");
        Text_Date.text = player.SaveDate;
    }
}
