using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    [Header("Âm thanh")]
    public AudioSource audioMusic;
    public AudioSource audioSound;
    public AudioClip backgroundMusicClip;
    public AudioClip upgradeSoundClip;
    public AudioClip roketSoundClip;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        SetMusicVolume();
    }
    

}
