using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IgnitionUI : ViewBase
{
    [SerializeField] private GameObject Panel_Ready;
    [SerializeField] private GameObject Panel_Start;
    [SerializeField] private AudioSource Audio_Ignition;

    [SerializeField] private List<Button> Button_Ignition = new List<Button>();

    private List<float> _playerTiming = new List<float>();
    private int _inputNum = 0;

    private List<int> _playerInput = new List<int>();
    private List<float> _playerIntervals = new List<float>();
    private List<float> _timing = new List<float>();
    private List<int> _sequences = new List<int>();

    private int _ignitionNum = 6;

    private void Awake()
    {
        for (int i = 0; i < Button_Ignition.Count; i++)
        {
            int index = i;
            Button_Ignition[i].onClick.AddListener(() => OnClickIgnition(index));
        }
    }

    private void OnEnable()
    {
        _timing.Clear();
        _sequences.Clear();
        _playerTiming.Clear();
        _playerInput.Clear();
        _playerIntervals.Clear();

        _inputNum = 0;

        for (int i = 0; i < _ignitionNum; i++)
        {
            int random = Random.Range(0, Button_Ignition.Count);
            _sequences.Add(random);

            float rand = Random.Range(0.1f, 1.5f);
            _timing.Add(rand);
        }

        Panel_Ready.gameObject.SetActive(false);
        Panel_Start.gameObject.SetActive(false);

        FlashStart().Forget();
    }

    private void OnClickIgnition(int index)
    {
        //if (index <= 28)
        //{
        //    AudioClip clip = await ResourceManager.Inst.LoadAsset<AudioClip>($"Sound/Minigame/{index}");

        //    Audio_Ignition.PlayOneShot(clip);
        //}

        _playerInput.Add(index);
        _playerTiming.Add(Time.time);

        int current = _playerInput.Count - 1;

        if (current > 0)
        {
            float interval = _playerTiming[current] - _playerTiming[current - 1];
            _playerIntervals.Add(interval);

            float correctInterval = _timing[current - 1];
            float playerInterval = _playerIntervals[current - 1];

            float diff = Mathf.Abs(correctInterval - playerInterval);

            if (diff > 2f)
            {
                UIManager.Inst.OpenFailPanel();
                return;
            }
        }

        _inputNum++;

        if (_inputNum == _ignitionNum)
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
    }

    private async UniTask FlashStart()
    {
        foreach (Button button in Button_Ignition)
        {
            button.interactable = false;
        }

        Panel_Start.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        Panel_Start.SetActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        for (int i = 0; i < _sequences.Count; i++)
        {
            int index = _sequences[i];
            Button_Ignition[index].image.color = Color.red;

            //string audioPath = $"Sound/Minigame/{}";
            //AudioClip clip = await ResourceManager.Inst.LoadAsset<AudioClip>(audioPath);

            //if (clip != null)
            //{
            //    Audio_Ignition.PlayOneShot(clip);
            //}

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            Button_Ignition[index].image.color = Color.gray;

            await UniTask.Delay(TimeSpan.FromSeconds(_timing[i]));
        }

        Panel_Ready.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        Panel_Ready.SetActive(false);

        foreach (Button button in Button_Ignition)
        {
            button.interactable = true;
        }
    }

    private bool CheckAnswer()
    {
        for (int i = 0; i < _sequences.Count; i++)
        {
            if (_playerInput[i] != _sequences[i])
            {
                return false;
            }
        }

        return true;
    }
}
