using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private PanelSlideController Panel_Current;
    [SerializeField] private float Speed_Gap = 0.3f;

    public async UniTask SwitchTo(PanelSlideController nextPanel, Vector2 exitDirection, Vector2 enterDirection)
    {
        if (nextPanel == Panel_Current)
        {
            return;
        }

        PanelSlideController prevPanel = Panel_Current;
        Panel_Current = nextPanel;

        prevPanel.SlideOut(exitDirection);

        await UniTask.Delay(TimeSpan.FromSeconds(Speed_Gap));

        nextPanel.SlideIn(enterDirection);
    }
}
