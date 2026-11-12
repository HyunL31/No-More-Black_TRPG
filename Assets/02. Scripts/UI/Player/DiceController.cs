using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{
    [SerializeField] private Transform Transform_Dice;

    [SerializeField] private List<Transform> Transform_Anchors = new List<Transform>();

    [SerializeField] private float Duration_Spin = 2.2f;
    [SerializeField] private float Duration_Rise = 0.35f;
    [SerializeField] private float Duration_Land = 0.5f;

    [SerializeField] private int Min_Extra = 3;
    [SerializeField] private int Max_Extra = 5;

    [SerializeField] private float Height = 1f;

    private Camera _diceCamera;

    private Sequence _roll;
    private Tween _float;

    private bool _isRolling;

    public Action OnRollComplete;

    private void Awake()
    {
        _diceCamera = GameObjectManager.Inst.GetDiceCamera();
    }

    private void OnDestroy()
    {
        _roll?.Kill();
        _float?.Kill();
    }

    public void Roll(int result)
    {
        if (_isRolling == true)
        {
            return;
        }

        Transform face = Transform_Anchors[result - 1];
        _isRolling = true;

        _roll?.Kill();

        Quaternion startRotation = Transform_Dice.rotation;
        Vector3 startPosition = Transform_Dice.position;

        Quaternion faceRotate = Quaternion.LookRotation(-_diceCamera.transform.forward, _diceCamera.transform.up);
        Quaternion delta = faceRotate * Quaternion.Inverse(face.rotation);
        Quaternion targetRotation = delta * startRotation;

        Vector3 axisA = Random.onUnitSphere;
        Vector3 axisB = Random.onUnitSphere;
        Vector3 axisC = Random.onUnitSphere;

        int turnsA = Random.Range(Min_Extra, Max_Extra + 1);
        int turnsB = Random.Range(2, 4);
        int turnsC = Random.Range(2, 4);

        if (Random.value < 0.5f) turnsA = -turnsA;
        if (Random.value < 0.5f) turnsB = -turnsB;
        if (Random.value < 0.5f) turnsC = -turnsC;

        float progress = 0f;

        _roll = DOTween.Sequence();
        _roll.Append(Transform_Dice.DOMoveY(startPosition.y + Height, Duration_Rise).SetEase(Ease.OutSine));
        _roll.Join(DOTween.To(() => progress, t =>
        {
            progress = t;

            Quaternion baseRot = Quaternion.Slerp(startRotation, targetRotation, t);
            Quaternion tumble = Quaternion.AngleAxis(360f * turnsA * t, axisA) * Quaternion.AngleAxis(360f * turnsB * t, axisB) * Quaternion.AngleAxis(360f * turnsC * t, axisC);

            Transform_Dice.rotation = tumble * baseRot;
        }, 1f, Duration_Spin).SetEase(Ease.OutQuart));

        _roll.Append(Transform_Dice.DOMoveY(startPosition.y, Duration_Land).SetEase(Ease.OutBounce));

        _roll.OnComplete(() =>
        {
            Transform_Dice.position = startPosition;
            Transform_Dice.rotation = targetRotation;

            _isRolling = false;
            OnRollComplete?.Invoke();
        });

        _roll.OnKill(() => _isRolling = false);
    }
}