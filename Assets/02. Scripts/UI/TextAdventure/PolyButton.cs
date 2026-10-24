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
    private string _culpritID = "";
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
            string culpritName = GameDataManager.Inst.GetData<Character>(_culpritID).Name;
            TextAdventureManager.Inst.OnAddLog?.Invoke("Player_01", culpritName);

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
        _culpritID = GameDataManager.Inst.GetData<Polygraph>(_polyID).CharacterID;

        Image_Culprit.sprite = await ResourceManager.Inst.LoadSprite($"Sprites/Character/{_culpritID}");
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

    public void InitState(Action<PolyButton> action)
    {
        _onClickButton -= action;
        _onClickButton += action;
    }
}
