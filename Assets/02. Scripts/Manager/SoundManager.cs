using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    [SerializeField] private AudioSource AudioSource_BGM;
    [SerializeField] private AudioSource AudioSource_SFX;

    public string GetSoundPath(string soundID)
    {
        return $"Sound/{soundID}";
    }

    public async UniTask LoadAudioClip(AudioSource source, string soundID, bool isLoop = false)
    {
        AudioClip clip = await ResourceManager.Inst.LoadAsset<AudioClip>(GetSoundPath(soundID));

        if (clip == null)
        {
            Debug.LogError($"사운드 로드 실패: {soundID}");
            return;
        }

        if (isLoop == true)
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
        else
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlaySFX(string soundID, bool isLoop = false)
    {
        LoadAudioClip(AudioSource_SFX, soundID).Forget();
    }

    public void PlayBGM(string soundID, bool isLoop = false)
    {
        LoadAudioClip(AudioSource_BGM, soundID, true).Forget();
    }

    public void StopSFX()
    {
        AudioSource_SFX.Stop();
    }

    public void StopBGM()
    {
        AudioSource_BGM.Stop();
    }
}
