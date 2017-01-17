using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public static AudioManager _instance = null;

    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }

    AudioSource _music;
    List<AudioSource> _soundList = new List<AudioSource>();
    int _soundSourceIndex = 0;
    int _soundSourceCount = 20;

    int _lastPlayFrameCount = -1;
    HashSet<string> _playingList = new HashSet<string>();

    //List<AudioSource> soundFreeList = new List<AudioSource>();
    //List<AudioSource> soundBusyList = new List<AudioSource>();

    public class SoundData
    {
        public string SoundName;
        public AudioClip SoundClip;
        public bool Loop;
        public int Volumn = 100;
    }

    Dictionary<string, SoundData> audioDic = new Dictionary<string, SoundData>();

    public bool MusicMute
    {
        get
        {
            int mute = PlayerPrefs.GetInt("MusicMute", 0);
            return mute > 0 ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt("MusicMute", value ? 1 : 0);
            _music.mute = value;
        }
    }

    public bool SoundMute
    {
        get
        {
            int mute = PlayerPrefs.GetInt("SoundMute", 0);
            return mute > 0 ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt("SoundMute", value ? 1 : 0);
            for (int i = 0; i < _soundList.Count; ++i)
            {
                _soundList[i].mute = value;
            }
            ActorAudio.SetSourcesMute(value);
        }
    }

    /// <summary>
    /// 待资源更新下载完成后，手动调用此函数 
    /// </summary>
    public static void CreateInstance()
    {
        GameObject go = new GameObject("AudioManager");
        //go.hideFlags = HideFlags.HideAndDontSave;

        GameObject.DontDestroyOnLoad(go);

        _instance = go.AddComponent<AudioManager>();

        _instance.Initialize();
    }

	// Use this for initialization
	void Start ()
    {
	}

    void OnEnable()
    {
        ButtonSound.RegistSoundHandler(HandleSound);
    }

    void OnDisable()
    {
        ButtonSound.UnRegistSoundHandler(HandleSound);
    }

    void HandleSound(string soundName, float soundDelay)
    {
        PlaySound(soundName, soundDelay);
    }

    void Initialize()
    {
        AddAudioSourceComponent();

        ReadSoundData();

        SetMuteState();
    }

    void AddAudioSourceComponent()
    {
        _music = gameObject.AddComponent<AudioSource>();
        _soundSourceCount = DefaultConfig.GetInt("SoundSourceCount");

        for (int i = 0; i < _soundSourceCount; ++i)
        {
            _soundList.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    void ReadSoundData()
    {
        CSVDataFile csvFile = new CSVDataFile();
        csvFile.ParseCSVFor("c_csv/c_audio_sound");

        for (int i = 2; i < csvFile.RowCount; ++i)
        {
            csvFile.SetRow(i);
            SoundData data = new SoundData();
            data.SoundName = csvFile.GetString("SoundName");
            data.Volumn = csvFile.GetInt("Volumn");
            data.Loop = csvFile.GetBool("Loop");

            string path = "Audio/" + csvFile.GetString("Sound");
            data.SoundClip = Resources.Load<AudioClip>(path);

            audioDic.Add(data.SoundName, data);
        }
    }

    void SetMuteState()
    {
        int mMute = PlayerPrefs.GetInt("MusicMute", 0);
        MusicMute = mMute > 0 ? true : false;

        int sMute = PlayerPrefs.GetInt("SoundMute", 0);
        SoundMute = sMute > 0 ? true : false;
    }

    bool AudioSourcePlay(AudioSource audioSource, SoundData soundData, float delay)
    {
        if (_lastPlayFrameCount == Time.frameCount)
        {
            if (_playingList.Contains(soundData.SoundName))
            {
                return false;
            }
        }
        else
        {
            _playingList.Clear();
        }

        _playingList.Add(soundData.SoundName);
        _lastPlayFrameCount = Time.frameCount;

        audioSource.clip = soundData.SoundClip;
        audioSource.volume = Mathf.Clamp01((float)soundData.Volumn / 100);
        audioSource.loop = soundData.Loop;

        if (delay > 0)
            audioSource.PlayDelayed(delay);
        else
            audioSource.Play();

        return true;
    }

    public void PlayMusic(string name, float delay = 0)
    {
        SoundData data = GetAudioClip(name);
        if (data != null)
        {
            AudioSourcePlay(_music, data, delay);
        }
    }

    public void StopMusic()
    {
        _music.Stop();
    }

    public void PlaySound(string name, float delay = 0)
    {
        SoundData data = GetAudioClip(name);
        if (data != null)
        {
            AudioSourcePlay(_soundList[_soundSourceIndex], data, delay);

            _soundSourceIndex++;
            _soundSourceIndex %= _soundSourceCount;
        }
    }

    public void StopSound()
    {
        for (int i =0; i < _soundList.Count; ++i)
        {
            _soundList[i].Stop();
        }
    }

    public SoundData GetAudioClip(string name)
    {
        SoundData data = null;
        audioDic.TryGetValue(name, out data);

        if (data == null)
        {
            Debug.LogError("Failed to found audio " + name);
        }

        return data;
    }
}
