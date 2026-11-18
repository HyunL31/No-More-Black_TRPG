using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{
    [SerializeField] private Transform Transform_Dice;
    [SerializeField] private List<Transform> Transform_Anchors = new List<Transform>();

    [SerializeField] private bool IsPlayer = false;

    [Header("파티클")]
    [SerializeField] private GameObject Effect_Bonus;
    [SerializeField] private GameObject Effect_Minus;

    public Action<int> OnPlusHit;
    public Action<int> OnMinusHit;
    public Action OnLandHit;
    public Action OnRollComplete;

    private Camera _diceCamera;

    private Quaternion _correctedRotation;
    private Vector3 _startPosition;
    private Vector3 _baseScale;

    private bool _isRolling;

    private Sequence _roll;

    private float _spinDuration = 3.5f;
    private float _riseDuration = 0.35f;
    private float _landDuration = 0.5f;
    private float _correctDuration = 1f;
    private float _height = 1f;

    private float _sweepRangeX = 1.5f;
    private float _sweepRangeY = 0.8f;

    private float _diceRadius = 0.8f;
    private float _hopHeight = 0.15f;
    private float _hopCount = 5f;

    private float _hopDuration = 0.6f;
    private float _pauseDuration = 0.4f;
    private float _hopArc = 0.8f;
    private float _hitSpinAngle = 360f;
    private float _hitSpinDuration = 0.5f;

    private Vector2 _plusHitOffset = new Vector2(-2f, -1.2f);
    private Vector2 _minusHitOffset = new Vector2(2f, -1.2f);

    private float _hitStop = 0.06f;
    private float _hitPunch = 0.3f;
    private float _recoilDis = 0.6f;
    private float _recoilHeight = 0.4f;
    private float _returnDuration = 0.6f;

    private void Awake()
    {
        _diceCamera = GameObjectManager.Inst.GetDiceCamera();

        _startPosition = Transform_Dice.position;
        _baseScale = Transform_Dice.localScale;
    }

    private void OnEnable()
    {
        Transform_Dice.position = _startPosition;

        Effect_Bonus.SetActive(false);
        Effect_Minus.SetActive(false);
    }

    private void OnDestroy()
    {
        _roll?.Kill();
        _roll = null;
        _isRolling = false;
    }

    public void Roll(int originResult, int plus, int minus)
    {
        if (IsPlayer == true)
        {
            PlayerRoll(originResult, plus, minus);
        }
        else
        {
            int finalResult = Mathf.Clamp(originResult + minus + plus, 1, 20);
            OpponentRoll(originResult, finalResult);
        }
    }

    private void PlayerRoll(int originResult, int plus, int minus)
    {
        if (_isRolling == true)
        {
            return;
        }

        int afterPlus = Mathf.Clamp(originResult + plus, 1, 20);
        int finalResult = Mathf.Clamp(afterPlus + minus, 1, 20);

        RollAnimation(originResult, finalResult);

        _roll.AppendInterval(_pauseDuration);

        AppendHit(true, afterPlus, () =>
        {
            OnPlusHit?.Invoke(plus);

            if (plus != 0)
            {
                Effect_Bonus.transform.position = GetHitPosition(true);
                Effect_Bonus.SetActive(true);
            }
        });

        AppendHit(false, finalResult, () =>
        {
            OnMinusHit?.Invoke(minus);

            if (minus != 0)
            {
                Effect_Minus.transform.position = GetHitPosition(false);
                Effect_Minus.SetActive(true);
            }
        });

        AppendReturn();

        _roll.OnComplete(() =>
        {
            Transform_Dice.position = _startPosition;
            Transform_Dice.rotation = _correctedRotation;
            Transform_Dice.localScale = _baseScale;

            Effect_Bonus.SetActive(false);
            Effect_Minus.SetActive(false);

            _isRolling = false;
            OnRollComplete?.Invoke();
        });

        _roll.OnKill(() =>
        {
            Transform_Dice.localScale = _baseScale;
            _isRolling = false;
        });
    }

    private void OpponentRoll(int originResult, int editResult)
    {
        if (_isRolling == true)
        {
            return;
        }

        RollAnimation(originResult, editResult);

        Tween land = Transform_Dice.DOMoveY(_startPosition.y - 1f, _landDuration).SetEase(Ease.OutBounce);

        float landStart;

        if (originResult != editResult)
        {
            _roll.Append(Transform_Dice.DORotateQuaternion(_correctedRotation, _correctDuration).SetEase(Ease.OutCubic));

            landStart = _roll.Duration() - _correctDuration;

            _roll.Join(land);
        }
        else
        {
            landStart = _roll.Duration();

            _roll.Append(land);
        }

        _roll.InsertCallback(landStart + _landDuration * 0.364f, () => PlayImpact(originResult, editResult));

        _roll.AppendInterval(0.2f);
        _roll.Append(Transform_Dice.DOMove(_startPosition, _returnDuration).SetEase(Ease.InOutCubic));

        _roll.OnComplete(() =>
        {
            Transform_Dice.position = _startPosition;
            Transform_Dice.rotation = _correctedRotation;

            Effect_Bonus.SetActive(false);
            Effect_Minus.SetActive(false);

            _isRolling = false;
            OnRollComplete?.Invoke();
        });

        _roll.OnKill(() => _isRolling = false);
    }

    private void RollAnimation(int originResult, int editResult)
    {
        Transform face = Transform_Anchors[originResult - 1];
        _isRolling = true;

        _roll?.Kill();

        Quaternion startRotation = Transform_Dice.rotation;

        Quaternion faceRotate = Quaternion.LookRotation(-_diceCamera.transform.forward, _diceCamera.transform.up);
        Quaternion delta = faceRotate * Quaternion.Inverse(face.rotation);
        Quaternion targetRotation = delta * startRotation;

        _correctedRotation = targetRotation;

        if (originResult != editResult)
        {
            if (editResult > 20)
            {
                editResult = 20;
            }
            else if (editResult < 1)
            {
                editResult = 1;
            }

            Transform editFace = Transform_Anchors[editResult - 1];

            Quaternion editDelta = faceRotate * Quaternion.Inverse(editFace.rotation);
            _correctedRotation = editDelta * startRotation;
        }

        Vector3 riseAxis = Random.onUnitSphere;

        float preSpin = 0.3f;
        float spinSpeed = 540f;
        float rollStart = preSpin + _riseDuration;

        Quaternion rollRotation = startRotation;

        _roll = DOTween.Sequence().SetLink(gameObject);

        _roll.Insert(0f, DOTween.To(() => 0f, x =>
        {
            rollRotation = Quaternion.AngleAxis(spinSpeed * Time.deltaTime, riseAxis) * rollRotation;
            Transform_Dice.rotation = rollRotation;
        }, 1f, rollStart).SetEase(Ease.Linear));

        _roll.Insert(preSpin, Transform_Dice.DOMoveY(_startPosition.y + _height, _riseDuration).SetEase(Ease.OutSine));

        Vector3 center = _startPosition + Vector3.up * _height;
        Vector3 camRight = _diceCamera.transform.right;
        Vector3 camUp = _diceCamera.transform.up;
        Vector3 toCamera = -_diceCamera.transform.forward;

        int pointCount = 40;
        Vector3[] path = new Vector3[pointCount + 1];

        float freqX = Random.Range(1.5f, 2.5f);
        float freqY = Random.Range(1.0f, 2.0f);
        float phaseX = Random.Range(0f, Mathf.PI * 2f);
        float phaseY = Random.Range(0f, Mathf.PI * 2f);

        for (int i = 0; i <= pointCount; i++)
        {
            float t = (float)i / pointCount;
            float envelope = Mathf.Sin(t * Mathf.PI);

            float x = Mathf.Sin(t * Mathf.PI * 2f * freqX + phaseX) * _sweepRangeX * envelope;
            float y = Mathf.Sin(t * Mathf.PI * 2f * freqY + phaseY) * _sweepRangeY * envelope;

            path[i] = center + camRight * x + camUp * y;
        }

        Vector3 lastPos = center;
        Tween move = null;

        move = Transform_Dice.DOPath(path, _spinDuration - rollStart, PathType.CatmullRom).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            Vector3 rawPos = Transform_Dice.position;
            Vector3 delta = rawPos - lastPos;
            lastPos = rawPos;

            float t = move.ElapsedPercentage();
            float dist = delta.magnitude;

            if (dist > 0.0001f)
            {
                Vector3 axis = Vector3.Cross(toCamera, delta / dist);
                float angle = dist / _diceRadius * Mathf.Rad2Deg;
                rollRotation = Quaternion.AngleAxis(angle, axis) * rollRotation;
            }

            float baseSpin = spinSpeed * Time.deltaTime * (1f - Mathf.InverseLerp(0.5f, 1f, t));
            rollRotation = Quaternion.AngleAxis(baseSpin, riseAxis) * rollRotation;

            float blend = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.65f, 1f, t));
            Transform_Dice.rotation = Quaternion.Slerp(rollRotation, targetRotation, blend);

            float hop = Mathf.Abs(Mathf.Sin(t * Mathf.PI * _hopCount)) * _hopHeight * (1f - t);
            Transform_Dice.position = rawPos + toCamera * hop;
        });

        _roll.Insert(rollStart, move);
    }

    private void AppendHit(bool isLeft, int targetResult, Action onHit)
    {
        Vector3 from = Vector3.zero;
        Vector3 to = Vector3.zero;
        Vector3 recoilDir = Vector3.zero;
        Quaternion spinStart = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        float spinDir = 1f;

        _roll.AppendCallback(() =>
        {
            from = Transform_Dice.position;
            to = GetHitPosition(isLeft);
            recoilDir = (_startPosition - to).normalized;
            spinStart = Transform_Dice.rotation;
            spinDir = Random.value < 0.5f ? -1f : 1f;
        });

        float travel = 0f;

        _roll.Append(DOTween.To(() => travel, p =>
        {
            travel = p;

            Vector3 pos = Vector3.Lerp(from, to, p);
            pos.y += Mathf.Sin(p * Mathf.PI) * _hopArc * (1f - p);
            Transform_Dice.position = pos;

            Transform_Dice.rotation = Quaternion.AngleAxis(720f * spinDir * Time.deltaTime, _diceCamera.transform.forward) * Transform_Dice.rotation;
        }, 1f, _hopDuration).SetEase(Ease.InCubic).OnStart(() => travel = 0f));

        _roll.AppendCallback(() =>
        {
            Transform_Dice.position = to;
            spinStart = Transform_Dice.rotation;

            targetRotation = GetFaceRotationFromCurrent(targetResult);

            onHit?.Invoke();

            Transform_Dice.DOKill();
            Transform_Dice.localScale = _baseScale;
            Transform_Dice.DOPunchScale(_baseScale * _hitPunch, 0.35f, 10, 0.6f);
        });

        _roll.AppendInterval(_hitStop);

        float recoil = 0f;
        _roll.Append(DOTween.To(() => recoil, p =>
        {
            recoil = p;

            Vector3 pos = to + recoilDir * (_recoilDis * p);
            pos.y += Mathf.Sin(p * Mathf.PI) * _recoilHeight;
            Transform_Dice.position = pos;

            Quaternion spin = Quaternion.AngleAxis(_hitSpinAngle * spinDir * p, _diceCamera.transform.forward) * spinStart;
            Transform_Dice.rotation = Quaternion.Slerp(spin, targetRotation, p);
        }, 1f, _hitSpinDuration).SetEase(Ease.OutCubic).OnStart(() => recoil = 0f));
    }

    private Quaternion GetFaceRotationFromCurrent(int result)
    {
        result = Mathf.Clamp(result, 1, 20);

        Quaternion faceRotate = Quaternion.LookRotation(-_diceCamera.transform.forward, _diceCamera.transform.up);
        Transform face = Transform_Anchors[result - 1];

        Quaternion delta = faceRotate * Quaternion.Inverse(face.rotation);
        return delta * Transform_Dice.rotation;
    }

    private void AppendReturn()
    {
        Vector3 from = Vector3.zero;

        _roll.AppendCallback(() => from = Transform_Dice.position);

        float progress = 0f;
        _roll.Append(DOTween.To(() => progress, p =>
        {
            progress = p;

            Vector3 pos = Vector3.Lerp(from, _startPosition, p);
            pos.y += Mathf.Sin(p * Mathf.PI) * _hopArc * 0.5f;
            Transform_Dice.position = pos;

            Transform_Dice.rotation = _correctedRotation;
        }, 1f, _returnDuration).SetEase(Ease.InOutCubic).OnStart(() => progress = 0f));
    }

    private Vector3 GetHitPosition(bool isLeft)
    {
        Vector2 offset = isLeft ? _plusHitOffset : _minusHitOffset;

        return _startPosition + _diceCamera.transform.right * offset.x + _diceCamera.transform.up * offset.y;
    }

    private void PlayImpact(int originResult, int editResult)
    {
        OnLandHit?.Invoke();

        if (originResult == editResult)
        {
            return;
        }

        if (editResult > originResult)
        {
            Effect_Bonus.SetActive(true);
        }
        else
        {
            Effect_Minus.SetActive(true);
        }
    }
}