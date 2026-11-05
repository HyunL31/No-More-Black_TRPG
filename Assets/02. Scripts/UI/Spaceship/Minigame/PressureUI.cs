using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PressureUI : ViewBase
{
    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI Text_Fuel;
    [SerializeField] private TextMeshProUGUI Text_Oxygen;
    [SerializeField] private TextMeshProUGUI Text_Cold;

    [Header("게이지")]
    [SerializeField] private Image Image_Fuel;
    [SerializeField] private Image Image_Oxygen;
    [SerializeField] private Image Image_Cold;

    [Header("다이얼")]
    [SerializeField] private Image Image_FuelDial;
    [SerializeField] private Image Image_OxygenDial;
    [SerializeField] private Image Image_ColdDial;
    [SerializeField] private DialController Dial_Fuel;
    [SerializeField] private DialController Dial_Oxygen;
    [SerializeField] private DialController Dial_Cold;

    [SerializeField] private Button Button_Confirm;

    private float _fuel;
    private float _oxygen;
    private float _cold;

    private void Awake()
    {
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnEnable()
    {
        Image_Fuel.fillAmount = 0;
        Image_Oxygen.fillAmount = 0;
        Image_Cold.fillAmount = 0;

        Dial_Fuel.OnChangeAngle += (angle) => ChangeGaugeColor(Image_Fuel, angle);
        Dial_Oxygen.OnChangeAngle += (angle) => ChangeGaugeColor(Image_Oxygen, angle);
        Dial_Cold.OnChangeAngle += (angle) => ChangeGaugeColor(Image_Cold, angle);

        SetRandomText();
    }

    private void OnClickConfirm()
    {
        if (CheckAnswer() == true)
        {
            UIManager.Inst.OpenWinPanel();
        }
        else
        {
            UIManager.Inst.OpenFailPanel();
        }
    }

    private void SetRandomText()
    {
        _fuel = Random.Range(0, 100);
        _oxygen = Random.Range(0, 100);
        _cold = Random.Range(0, 100);

        Text_Fuel.text = $"연료 압력: {_fuel} %";
        Text_Oxygen.text = $"산화제 압력: {_oxygen}%";
        Text_Cold.text = $"냉각 / 헬륨 압력: {_cold}%";
    }

    private void ChangeGaugeColor(Image gauge, float angle)
    {
        gauge.fillAmount = angle / 360f;

        if (gauge.fillAmount <= 0.33f)
        {
            gauge.color = Color.red;
        }
        else if (gauge.fillAmount <= 0.66f)
        {
            gauge.color = Color.yellow;
        }
        else
        {
            gauge.color = Color.green;
        }
    }

    private bool CheckAnswer()
    {
        if (IsNear(Image_Fuel.fillAmount, _fuel / 100) && IsNear(Image_Oxygen.fillAmount, _oxygen / 100) && IsNear(Image_Cold.fillAmount, _cold / 100))
        {
            return true;
        }

        return false;
    }

    private bool IsNear(float current, float answer)
    {
        if (Mathf.Abs(current - answer) <= 0.1)
        {
            return true;
        }

        return false;
    }
}
