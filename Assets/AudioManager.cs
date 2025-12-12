using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;

    public AudioSource audioMusic;
    public AudioSource audioSoundPlayer;
    public AudioSource audioSound;
    [Header("Audio Clips")]

    [Header("Nhạc nền")]
    public AudioClip backgroundMusicClip;
    [Header("Nâng cấp")]
    public AudioClip upgradeSoundClip;
    [Header("Vụ nổ")]
    public AudioClip explosionSoundClip;
    [Header("Bấm nút")]
    public AudioClip buttonSoundClip;
    [Header("Thoát ")]
    public AudioClip exitSoundClip;
    [Header("Âm thanh lúc chơi")]
    public AudioClip gameplaySoundClip;
    [Header("Âm thanh khi chiến thắng")]
    public AudioClip victorySoundClip;
    [Header("Âm thanh khi thua cuộc")]
    public AudioClip defeatSoundClip;
    [Header("Âm thanh khi thu thập tiền")]
    public AudioClip collectMoneySoundClip;
    [Header("Âm thanh khi thu thập kim cương")]
    public AudioClip collectDiamondSoundClip;
    [Header("Âm thanh khi sử dụng booster")]
    public AudioClip boosterSoundClip;
    [Header("Âm thanh khi va chạm chướng ngai vật")]
    public AudioClip obstacleCollisionSoundClip;
    [Header("Âm thanh khi va chạm Rocket")]
    public AudioClip rocketCollisionSoundClip;
    [Header("Âm thanh khi hạ cánh")]
    public AudioClip landingSoundClip;
    [Header("Âm thanh khi hạ cánh perfect")]
    public AudioClip perfectLandingSoundClip;
    [Header("Âm thanh max power")]
    public AudioClip maxPowerSoundClip;
    [Header("Âm thanh mở khóa map mới")]
    public AudioClip unlockMapSoundClip;
    [Header("Âm thanh mở khóa máy bay mới")]
    public AudioClip unlockPlaneSoundClip;
    [Header("Âm thanh bắt đầu cất cánh")]
    public AudioClip takeOffSoundClip;
    [Header("Âm thanh máy bay đang rơi")]
    public AudioClip fallingSoundClip;
    [Header("Âm thanh đếm tiền")]
    public AudioClip countMoneySoundClip;
    [Header("Âm thanh LuckyWheel")]
    public AudioClip luckyWheelSoundClip;
    [Header("Âm thanh khi đạt Perfect Angle")]
    public AudioClip perfectAngleSoundClip;
    [Header("Âm thanh khi không đạt Perfect Angle")]
    public AudioClip notPerfectAngleSoundClip;
    [Header("Âm thanh khi đạt nhận thưởng LuckyWheel")]
    public AudioClip rewardLuckyWheelSoundClip;
    [Header("Âm thanh khi left right trong Shop")]
    public AudioClip leftRightShopSoundClip;
    [Header("Âm thanh khi đâm vào Bonus")]
    public AudioClip crashBonusSoundClip;
    public bool isMusicOn = true;
    public bool isSoundOn = true;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // DontDestroyOnLoad(gameObject);
        LoadVolume();
    }

    void Start()
    {
        audioMusic.volume = 1f;       // giảm nhạc nền
        audioSound.volume = 0.5f;         // sound max
        audioSoundPlayer.volume = 0.5f;

        SetMusicVolume();
        SetSoundVolume();
    }


    public void SetMusicVolume()
    {
        if (isMusicOn)      
        {
            audioMusic.clip = backgroundMusicClip;
            audioMusic.loop = true;

            if (!audioMusic.isPlaying)
                audioMusic.Play();
        }
        else
        {
            if (audioMusic.isPlaying)
                audioMusic.Stop();
        }
    }

    public void SetSoundVolume()
    {
        audioSound.volume = isSoundOn ? 1f : 0f;
        audioSoundPlayer.volume = isSoundOn ? 1f : 0f;

        if (isSoundOn)
        {
            if (audioSoundPlayer.clip != null && !audioSoundPlayer.isPlaying)
                audioSoundPlayer.Play();
        }
        else
        {
            audioSoundPlayer.Stop();
        }
    }


    public void LoadVolume()
    {
        isSoundOn = PlayerPrefs.GetInt("SoundVolume", 1) == 1;

        isMusicOn = PlayerPrefs.GetInt("MusicVolume", 1) == 1;

        SetSoundVolume();
        SetMusicVolume();
    }

    public void buttonSound()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("SoundVolume", isSoundOn ? 1 : 0);

        SetSoundVolume();
    }

    public void buttonMusic()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicVolume", isMusicOn ? 1 : 0);

        SetMusicVolume();
    }

     public void PlaySound(AudioClip clip)
    {
        if (isSoundOn && clip != null)
            audioSound.PlayOneShot(clip);
    }

    public void PlayPlayerSound(AudioClip clip)
    {
        if (isSoundOn && clip != null)
            audioSoundPlayer.clip = clip;
            audioSoundPlayer.loop = true;
            audioSoundPlayer.Play();
    }

    public void StopPlayerSound()
    {
        if (audioSoundPlayer.isPlaying)
            audioSoundPlayer.Stop();
    }
    public  void StopSoundBackground()
    {
        if (audioMusic.isPlaying)
            audioMusic.Stop();
    }
}
