using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer
{
    public SFXSource source{
        get{
            if(_source == null)
            {
                _source = Audio.GetSource();
            }
            return _source;
        }
    }
    private SFXSource _source;
    private Coroutine disableSelfRoutine;
    
    public Channel channel;

    public AudioPlayer(AudioClip clip)
    {
        source.audioSource.clip = clip;
        source.gameObject.name = "SFXSource (" + clip.name + ")";
    }

    public void Play()
    {
        if(SaveManager.currentSave.sfx)
        {
            source.audioSource.Play();
        }
            

        if(!source.audioSource.loop && source.isActiveAndEnabled)
            disableSelfRoutine = source.StartCoroutine(DisableSelf());
    }

    private IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(source.audioSource.clip.length);
        disableSelfRoutine = null;
        Kill();
    }

    public void Kill()
    {
        Audio.KillAudioObject(this);
        SetImmortal(false);

        if (_source == null || _source.audioSource == null)
            return;

        if (disableSelfRoutine != null)
            source.StopCoroutine(disableSelfRoutine);

        source.audioSource.Stop();
        source.audioSource.time = 0f;

        source.gameObject.SetActive(false);
    }

    public AudioPlayer SetPitch(float pitch)
    {
        source.audioSource.pitch = Mathf.Clamp(pitch,-3f, 3f);
        return this;
    }

    public AudioPlayer SetVolume(float volume)
    {
        source.audioSource.volume = volume;
        return this;
    }

    public AudioPlayer SetMute(bool mute)
    {
        source.audioSource.mute = mute;
        return this;
    }

    public AudioPlayer SetLoop(bool loop)
    {
        source.audioSource.loop = loop;
        return this;
    }

    public AudioPlayer SetChannel(Channel channel)
    {
        this.channel = channel;
        return this;
    }

    public AudioPlayer SetImmortal(bool dontDestroyOnLoad)
    {
        if(dontDestroyOnLoad)
            GameObject.DontDestroyOnLoad(source.gameObject);
        else
            SceneManager.MoveGameObjectToScene(source.gameObject, SceneManager.GetActiveScene());

        return this;
    }

    public AudioPlayer SetPosition(float time)
    {
        if(time > source.audioSource.clip.length)
            Debug.LogError(time + " is larger than the length of the clip!");

        source.audioSource.time = time;
        return this;
    }
}
