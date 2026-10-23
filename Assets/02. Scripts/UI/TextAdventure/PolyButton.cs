using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PolyButton : MonoBehaviour
{
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Button Button_Poly;
    [SerializeField] private Image Image_Culprit;

    private string _polyID = "";
    private bool _isSelected = false;

    private Color _normalColor = Color.white;
    private Color _selectColor = Color.red;

    private Action<PolyButton> _onClickButton;

    private void OnEnable()
    {
        Button_Poly.onClick.AddListener(OnClickCulprit);

        _isSelected = false;
        SetFrame();
    }

    private void OnClickCulprit()
    {
        _onClickButton?.Invoke(this);

        if (_isSelected == false)
        {
            _isSelected = true;
            SetFrame();
        }
        else
        {
            string returnID = GameDataManager.Inst.GetData<Polygraph>(_polyID).NextID;
            TextAdventureManager.Inst.SetCurrentID(returnID);

            UIManager.Inst.OpenDialogueUI();
            UIManager.Inst.ClosePolygraphUI();
        }
    }

    private void SetFrame()
    {
        if (_isSelected == true)
        {
            Image_Frame.color = _selectColor;
        }
        else
        {
            Image_Frame.color = _normalColor;
        }
    }

    private void SetPolyID(string polyID)
    {
        _polyID = polyID;
    }

    private async UniTask SetPolyImage()
    {
        string culprit = GameDataManager.Inst.GetData<Polygraph>(_polyID).CharacterID;

        Image_Culprit.sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{culprit}");
    }

    public Image GetCulpritImage()
    {
        return Image_Culprit;
    }

    public void Deselect()
    {
        _isSelected = false;
        SetFrame();
    }

    public void InitButton(string polyID)
    {
        SetPolyID(polyID);
        SetPolyImage().Forget();
        Deselect();
    }

    public void InitState(Action<PolyButton> callback)
    {
        _onClickButton -= callback;
        _onClickButton += callback;
    }
}
