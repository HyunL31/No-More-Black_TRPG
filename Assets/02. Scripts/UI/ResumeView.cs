using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeView : ViewBase
{
    [Header("Player")]
    [SerializeField] private Image Image_Player;
    [SerializeField] private TextMeshProUGUI Text_RegisterCode;
    [SerializeField] private TextMeshProUGUI Text_Job;
    [SerializeField] private TextMeshProUGUI Text_PlayerName;

    [Header("Operation")]
    [SerializeField] private GameObject Slot_Guage;
    [SerializeField] private Transform Parent_Stamina;
    [SerializeField] private Transform Parent_Crazy;
    [SerializeField] private Transform Parent_Oblivion;

    [Header("Ideology")]
    [SerializeField] private Image Image_LeftIdeology;
    [SerializeField] private Image Image_RightIdeology;
    [SerializeField] private TextMeshProUGUI Text_Ideology;

    [Header("Relationship")]
    [SerializeField] private Button Button_Left;
    [SerializeField] private Button Button_Right;

    [SerializeField] private Image Image_PrevNPC;
    [SerializeField] private Image Image_NextNPC;
    [SerializeField] private Image Image_CurrentNPC;
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCName;
    [SerializeField] private TextMeshProUGUI Text_CurrentNPCRelationship;
}