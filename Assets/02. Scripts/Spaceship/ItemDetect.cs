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

    private float _requireDis = 1.5f;

    private float _checkInterval = 0.15f;
    private float _checkTimer;

    private void Start()
    {
        Outline.enabled = false;
        _player = GameObjectManager.Inst.GetPlayer().gameObject;

        UIBase uiBase = UIManager.Inst.GetOpenedUI(UIRoot.Main, UIType.MainUI);
        
        if (uiBase is MainUI mainUI)
        {
            mainUI.GetMinimap().RegisterClue(transform);
        }
    }

    private void Update()
    {
        _checkTimer += Time.deltaTime;

        if (_checkTimer < _checkInterval)
        {
            return;
        }

        _checkTimer = 0;
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

            UIBase uiBase = UIManager.Inst.GetOpenedUI(UIRoot.Main, UIType.MainUI);

            if (uiBase is MainUI mainUI)
            {
                mainUI.GetMinimap().UnregisterClue(transform);
            }
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
