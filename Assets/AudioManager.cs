using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;

    [Header("Âm thanh")]
    public AudioSource audioMusic;
    public AudioSource audioSound;
    public AudioClip backgroundMusicClip;
    public AudioClip upgradeSoundClip;
    public AudioClip roketSoundClip;
    public bool isMusicOn = true;
    public bool isSoundOn = true;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        LoadVolume();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetMusicVolume();
        SetSoundVolume();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMusicVolume()
    {
        if (isMusicOn)
        {
            audioMusic.clip = backgroundMusicClip;
            audioMusic.loop = true;
            audioMusic.Play();
        }
        else
        {
            audioMusic.Stop();
        }
        
    }
    public void SetSoundVolume()
    {
        if (isSoundOn)
        {
            audioSound.volume = 1f;
        }
        else
        {
            audioSound.volume = 0f;
        }
    }

    public void LoadVolume()
    {
        isSoundOn = PlayerPrefs.GetInt("SoundVolume", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("MusicVolume", 1) == 1;
        SetSoundVolume();
        SetMusicVolume();
    }
    

}
