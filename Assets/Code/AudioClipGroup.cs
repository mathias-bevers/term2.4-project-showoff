using System.Collections.Generic;
using UnityEngine;

public class AudioClipGroup : MonoBehaviour
{
    [SerializeField] List<AudioDelay> audioDelays;

    float timer = 0;

    public bool pausesOnGamePause = false;
    public bool playOnAwake = true;
    public bool loop = false;
    public bool destroyOnEnd = false;
    bool lastLoop = false;
#if UNITY_EDITOR
    [Tooltip("DEBUG KEY! press this to start playing the audio")]
    [SerializeField]
    KeyCode enableKey = KeyCode.None;
    [SerializeField]
    [Tooltip("DEBUG KEY! press this to reset playing the audio")]
    KeyCode resetKey = KeyCode.None;
#endif
    bool playing = false;

    float _actualVolume = 80;
    public float internalVolume
    {
        get => _actualVolume; set
        {
            _actualVolume = Utils.Map(value, 0, 100, 0, 1);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < audioDelays.Count; i++)
        {
            if (audioDelays[i].audioSource == null) continue;
            audioDelays[i].audioSource.volume = audioDelays[i].startVolume;
        }
    }

    private void OnEnable()
    {
        StopAudio();
    }

    private void Awake()
    {
        StopAudio();

        if (playOnAwake && !playing)
            playing = true;
    }

    void StopAudio()
    {
        for (int i = 0; i < audioDelays.Count; i++)
        {
            if (audioDelays[i].audioSource == null) continue;
            audioDelays[i].audioSource.playOnAwake = false;
            audioDelays[i].SetStartVolume();
            //audioDelays[i].audioSource.Play();
            //audioDelays[i].audioSource.Pause();
            audioDelays[i].audioSource.gameObject.SetActive(false);

        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        ResetValues();
        playing = true;
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        ResetValues();
    }

    void ResetValues()
    {
        timer = 0;
        playing = false;
        for (int i = 0; i < audioDelays.Count; i++)
        {
            audioDelays[i].audioSource.Stop();
            audioDelays[i].hasPlayed = false;
            audioDelays[i].hasFinished = false;
            audioDelays[i].audioSource.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        internalVolume = 100;

#if UNITY_EDITOR
        if (Input.GetKeyDown(resetKey)) ResetValues();
        if (!playing) if (Input.GetKeyDown(enableKey)) Play();
#endif
        if (!playing) return;
        /*if (((PauseHandler.Instance.isPaused ||Time.timeScale == 0) && pausesOnGamePause) || paused)
        {
            for (int i = 0; i < audioDelays.Count; i++)
            {
                if (audioDelays[i].audioSource == null) continue;
                audioDelays[i].audioSource.Pause();
            }
            return;
        }*/
        timer += Time.unscaledDeltaTime;
        bool allHasPlayed = true;

        bool any = false;
        for (int i = 0; i < audioDelays.Count; i++)
        {
            AudioDelay delay = audioDelays[i];
            if (delay.audioSource == null) continue;
            if (timer >= delay.delay)
            {
                any = true;
                delay.audioSource.volume = delay.startVolume * _actualVolume;
                delay.audioSource.gameObject.SetActive(true);
                float pTime = delay.audioSource.time;
                delay.audioSource.UnPause();
                if (!delay.audioSource.isPlaying)
                {
                    if (!delay.hasPlayed)
                    {
                        delay.audioSource.Play();
                        allHasPlayed = false;
                    }
                    else
                    {
                        delay.hasFinished = true;
                    }
                }
                else
                    delay.hasPlayed = true;

            }
            if (!delay.hasFinished)
                allHasPlayed = false;
        }
        if (!any)
            allHasPlayed = false;

        if (lastLoop != loop)
        {
            lastLoop = loop;
            allHasPlayed = true;
        }

        if (allHasPlayed)
        {
            Stop();
            if (destroyOnEnd)
                Destroy(gameObject);
            if (loop)
                Play();
        }
    }
}

[System.Serializable]
public class AudioDelay
{
    [SerializeField]
    public AudioSource audioSource;
    [SerializeField]
    public float delay;
    [HideInInspector]
    public float startVolume;
    [HideInInspector]
    public bool hasPlayed = false;
    [HideInInspector]
    public bool hasFinished = false;

    public AudioDelay(AudioSource audioSource, float delay)
    {
        this.audioSource = audioSource;
        this.delay = delay;
        startVolume = 1;
        SetStartVolume();
    }

    public void SetStartVolume()
    {
        if (audioSource == null)
            startVolume = 0;
        else
            startVolume = audioSource.volume;
    }
}
