using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSelector : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> Texts;
    [SerializeField] private List<Button> Buttons;
    [SerializeField] private float Size_OriginText = 70f;
    [SerializeField] private float Size_BigText = 76f;

    private int _currentIndex = 0;

    private void OnEnable()
    {
        Init();

        _currentIndex = 0;
        ChangeButton(_currentIndex);
    }

    private void Update()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll < 0 || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ReturnButton(_currentIndex);

            if (_currentIndex < Texts.Count - 1)
            {
                _currentIndex++;
            }

            ChangeButton(_currentIndex);
        }
        else if (mouseScroll > 0 || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ReturnButton(_currentIndex);

            if (_currentIndex > 0)
            {
                _currentIndex--;
            }

            ChangeButton(_currentIndex);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Buttons[_currentIndex].onClick.Invoke();
        }
    }

    private void ChangeButton(int index)
    {
        Texts[index].fontSize = Size_BigText;
        Texts[index].color = Color.white;
    }

    private void ReturnButton(int index)
    {
        Texts[index].fontSize = Size_OriginText;
        Texts[index].color = Color.gray;
    }

    private void Init()
    {
        for (int i = 0; i < Texts.Count; i++)
        {
            ReturnButton(i);
        }
    }

    public void SelectButton(int index)
    {
        if (index < 0 || index >= Texts.Count)
        {
            return;
        }

        if (_currentIndex == index)
        {
            return;
        }

        ReturnButton(_currentIndex);
        _currentIndex = index;
        ChangeButton(_currentIndex);
    }
}
