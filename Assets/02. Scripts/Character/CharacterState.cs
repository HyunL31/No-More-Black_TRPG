using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;
using System.Threading;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic SkeletonGraphic;
    [SerializeField] private InspectorCharacter InspectorCharacter;

    private string _currentAnim;
    private string _currentSkin;

    private Skeleton _skeleton;
    private SkeletonData _skeletonData;
    private Skin _combinedSkin;

    private CancellationTokenSource _blinkToken;

    private Color _dark = new Color(0.45f, 0.45f, 0.45f, 1f);

    private void Awake()
    {
        _skeleton = SkeletonGraphic.Skeleton;
        _skeletonData = _skeleton.Data;
        _combinedSkin = new Skin("CombinedSkin");

        InspectorCharacter.enabled = false;
    }

    private void OnDestroy()
    {
        CancelBlink();
    }

    private void ChangeAnimation(string currentAnim)
    {
        if (_currentAnim != currentAnim && !string.IsNullOrEmpty(currentAnim))
        {
            SkeletonGraphic.AnimationState.SetAnimation(0, currentAnim, true);
            _currentAnim = currentAnim;
        }

        if (currentAnim == "none" || currentAnim == "basic")
        {
            if (_blinkToken == null || _blinkToken.IsCancellationRequested)
            {
                BlinkRoutine().Forget();
            }
        }
        else
        {
            CancelBlink();
        }

        SkeletonGraphic.Update(0f);
    }

    private void ChangeSkin(string currentSkin)
    {
        if (_currentSkin == currentSkin || string.IsNullOrEmpty(currentSkin))
        {
            return;
        }

        _currentSkin = currentSkin;

        if (_skeletonData.DefaultSkin != null)
        {
            _combinedSkin.AddSkin(_skeletonData.DefaultSkin);
        }

        Skin eyeSkin = _skeletonData.FindSkin("eye/" + currentSkin);
        if (eyeSkin != null)
        {
            _combinedSkin.AddSkin(eyeSkin);
        }

        Skin mouthSkin = _skeletonData.FindSkin("mouth/" + currentSkin);
        if (mouthSkin != null)
        {
            _combinedSkin.AddSkin(mouthSkin);
        }

        SkeletonGraphic.OverrideTexture = null;
        _skeleton.SetSkin(_combinedSkin);
        _skeleton.SetSlotsToSetupPose();
    }

    public void ApplyEmotion(string currentAnim, string currentSkin)
    {
        ChangeAnimation(currentAnim);
        ChangeSkin(currentSkin);
    }

    private async UniTask BlinkRoutine()
    {
        _blinkToken?.Cancel();
        _blinkToken = new CancellationTokenSource();

        var token = _blinkToken.Token;

        while (!token.IsCancellationRequested)
        {
            float delay = Random.Range(0.5f, 3.5f);

            await UniTask.Delay(Mathf.RoundToInt(delay * 1000), cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            if (SkeletonGraphic != null)
            {
                SkeletonGraphic.AnimationState.SetAnimation(1, "eye", false);
            }
        }
    }

    public void SetWhite()
    {
        SkeletonGraphic.color = Color.white;
    }

    public void SetDark()
    {
        SkeletonGraphic.color = _dark;
    }

    private void CancelBlink()
    {
        if (_blinkToken != null)
        {
            _blinkToken.Cancel();
            _blinkToken.Dispose();
            _blinkToken = null;
        }
    }
}