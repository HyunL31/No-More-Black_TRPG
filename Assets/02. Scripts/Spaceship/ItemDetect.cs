using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Outline Outline;
    [SerializeField] private Texture2D Cursor_Default;
    [SerializeField] private Texture2D Cursor_Clue;
    [SerializeField] private string ItemID;

    private GameObject _player;
    private bool _canInteract;
    private bool _isCollected = false;

    private float _requireDis = 3f;

    private void Start()
    {
        Outline.enabled = false;
        _player = GameObjectManager.Inst.GetPlayer();
    }

    private void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        float dist = Vector2.Distance(_player.transform.position, this.transform.position);

        if (dist > _requireDis)
        {
            _canInteract = false;
            Outline.enabled = false;
        }
        else
        {
            _canInteract = true;
            Outline.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_canInteract == false)
        {
            return;
        }

        if (_isCollected == false)
        {
            if (SaveManager.Inst.PlayerModel.Inventory.ContainsKey(ItemID) == false)
            {
                SaveManager.Inst.PlayerModel.Inventory.Add(ItemID, 1);
            }
            else
            {
                SaveManager.Inst.PlayerModel.Inventory[ItemID]++;
            }

            SaveManager.Inst.PlayerModel.CollectedItem.Add(ItemID);

            _isCollected = true;
        }

        UIManager.Inst.OpenItemUI(ItemID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_canInteract == true)
        {
            Cursor.SetCursor(Cursor_Clue, Vector2.zero, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(Cursor_Default, Vector2.zero, CursorMode.Auto);
    }
}
