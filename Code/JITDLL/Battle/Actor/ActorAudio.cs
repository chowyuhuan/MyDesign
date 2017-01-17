using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

public class ActorAudio : CompBase
{
    public static List<AudioSource> _gSourceList = new List<AudioSource>();
    AudioSource _source;
    public float Speed
    {
        set 
        {
            _source.pitch = value;
        }
        get
        {
            return _source.pitch;
        }
    }

    public override void Init(Actor a)
    {
        base.Init(a);
        _source = a.gameObject.AddComponent<AudioSource>();
        _source.mute = AudioManager.Instance.SoundMute;
        _gSourceList.Add(_source);
    }

    public void Play(string name)
    {
        AudioManager.SoundData data = AudioManager.Instance.GetAudioClip(name);
        _source.clip = data.SoundClip;
        _source.volume = Mathf.Clamp01((float)data.Volumn / 100);
        _source.loop = data.Loop;
        _source.Play();
    }

    public static void SetSourcesMute(bool setting)
    {
        for (int i = 0; i < _gSourceList.Count; ++i)
        {
            if (_gSourceList[i] != null)
            {
                _gSourceList[i].mute = setting;
            }
        }
    }
    public static void ClearSources()
    {
        _gSourceList.Clear();
    }
}
