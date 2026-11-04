using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private Button Button_GermUI;

    private void Awake()
    {
        Button_GermUI.onClick.AddListener(OnClickOpenGermUI);
    }

    private void OnClickOpenGermUI()
    {
        UIManager.Inst.OpenGermUI();
    }
}
