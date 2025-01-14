using UnityEngine;
using System.Collections;
/*
 * This is SoundManager
 * In other script, you just need to call SoundManager.PlaySfx(AudioClip) to play the sound
*/
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioClip beginSoundInMainMenu;
    [Tooltip("Play music clip when start")]
    public AudioClip musicsGame;
    public AudioClip BossMusicClip;
    [Range(0, 1)]
    public float musicsGameVolume = 0.3f;

    [Tooltip("Place the sound in this to call it in another script by: SoundManager.PlaySfx(soundname);")]
    public AudioClip soundClick;
    public AudioClip coinCollect;
    [Header("Game State")]
    public AudioClip soundFail;
    public AudioClip soundPause;
    public AudioClip soundShowStagePanel;

    [Header("ADD/UPGRADE ARCHER")]
    public AudioClip soundAddArcher;
    public AudioClip soundUpgradeArcher;

    [Header("Shop")]
    public AudioClip soundPurchased;
    public AudioClip soundUpgrade;
    public AudioClip soundNotEnoughCoin;

    [Header("Victory")]
    public AudioClip soundVictoryPanel;
    public AudioClip soundStar1;
    public AudioClip soundStar2;
    public AudioClip soundStar3;

    private AudioSource musicAudio;
    private AudioSource soundFx;

    [Header("BOOST ITEM")]
    public AudioClip BTsoundOpen;
    public AudioClip BTsoundHide;
    public AudioClip BTsoundUseBoost;

    [Header("OTHER")]
    public AudioClip soundTimeUp;
    public AudioClip soundTimeDown;
    //public AudioClip switchPlayerSound;

    //public AudioClip soundCheckpoint;
    public void PauseMusic(bool isPause)
    {
        if (isPause)
            Instance.musicAudio.mute = true;
        //			Instance.musicAudio.Pause ();
        else
            Instance.musicAudio.mute = false;
        //			Instance.musicAudio.UnPause ();
    }
    //GET and SET
    public static float MusicVolume
    {

        set { Instance.musicAudio.volume = value; }
        get { return Instance.musicAudio.volume; }
    }
    public static float SoundVolume
    {
        set { Instance.soundFx.volume = value; }
        get { return Instance.soundFx.volume; }
    }
    // Use this for initialization
    void Awake()
    {
        Instance = this;
        musicAudio = gameObject.AddComponent<AudioSource>();
        musicAudio.loop = true;
        musicAudio.volume = 0.5f;
        soundFx = gameObject.AddComponent<AudioSource>();
    }
    void Start()
    {
        //		//Check auido and sound
        //		if (!GlobalValue.isMusic)
        //			musicAudio.volume = 0;
        //		if (!GlobalValue.isSound)
        //			soundFx.volume = 0;
        //		PlayMusic(musicsGame,musicsGameVolume);
        //		//Check auido and sound

        PlayMusic(musicsGame, musicsGameVolume);
    }

    public static void Click()
    {
        PlaySfx(Instance.soundClick);
    }
    
    public void ClickBut()
    {
        PlaySfx(soundClick);
    }

    public static void PlaySfx(AudioClip clip)
    {
        if (Instance != null)
        {
            Instance.PlaySound(clip, Instance.soundFx);
        }


    }

    public static void PlaySfx(AudioClip clip, float volume)
    {
        if (Instance != null)
            Instance.PlaySound(clip, Instance.soundFx, volume);
    }

    public static void PlaySfx(AudioClip[] clips)
    {
        if (Instance != null && clips.Length > 0)
            Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx);
    }

    public static void PlaySfx(AudioClip[] clips, float volume)
    {
        if (Instance != null && clips.Length > 0)
            Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx, volume);
    }

    public static void PlayMusic(AudioClip clip)
    {
        Instance.PlaySound(clip, Instance.musicAudio);
    }

    public static void PlayMusic(AudioClip clip, float volume)
    {
        Instance.PlaySound(clip, Instance.musicAudio, volume);
    }

    private void PlaySound(AudioClip clip, AudioSource audioOut)
    {
        if (clip == null)
        {
            //			Debug.Log ("There are no audio file to play", gameObject);
            return;
        }

        if (Instance == null)
            return;

        if (audioOut == musicAudio)
        {
            audioOut.clip = clip;
            audioOut.Play();
        }
        else
            audioOut.PlayOneShot(clip, SoundVolume);
    }

    private void PlaySound(AudioClip clip, AudioSource audioOut, float volume)
    {
        if (clip == null)
        {
            //			Debug.Log ("There are no audio file to play", gameObject);
            return;
        }

        if (audioOut == musicAudio)
        {
            //if (!GlobalValue.isMusic) return;
            audioOut.volume = GlobalValue.isMusic ? volume : 0;
            audioOut.clip = clip;
            audioOut.Play();
        }
        else
        {
            if (!GlobalValue.isSound) return;
            audioOut.PlayOneShot(clip, SoundVolume * volume);
        }
    }
}
