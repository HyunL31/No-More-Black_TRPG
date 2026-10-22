using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygraphUI : ViewBase
{
    [SerializeField] private Button Button_Hint;
    [SerializeField] private List<Button> Button_Polys;

    private void Awake()
    {
        Button_Hint.onClick.AddListener(OnClickHint);
    }

    private void OnEnable()
    {
        
    }

    private void OnClickHint()
    {

    }

    private void SetPolyButtons()
    {

    }
}
