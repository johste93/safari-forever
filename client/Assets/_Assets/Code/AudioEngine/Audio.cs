using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public GameObject SFXSourcePrefab;

    private const string RESOURCE_PATH = "";
	private const string FILENAME = "AudioEngine";

    private static string path { get { return RESOURCE_PATH + FILENAME; } }

    private const int maxQued = 6;
    
    private static Audio _instance;
    private static Audio instance
    {
        get{
            if(_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameObject>(path)).GetComponent<Audio>();
                _instance.StartCoroutine(_instance.PlayQueued());
            }
            
            return _instance;
        }
    }

    private ObjectPool sourcePool = new ObjectPool();
    private Queue<AudioPlayer> queuedAudioObject = new Queue<AudioPlayer>();
    private List<AudioPlayer> livingAudioObjects = new List<AudioPlayer>();

    public static AudioPlayer Play(AudioEntry entry, Channel channel)
    {
        if(channel == Channel.Game && instance.queuedAudioObject.Count > maxQued)
            return null;

        AudioPlayer newAudioObject = new AudioPlayer(entry.randomClip);

        newAudioObject.SetVolume(entry.defaultVolume);
        newAudioObject.SetPitch(entry.defaultPitch);
        newAudioObject.SetLoop(entry.loop);
        newAudioObject.SetChannel(channel);

        instance.queuedAudioObject.Enqueue(newAudioObject);
        instance.livingAudioObjects.Add(newAudioObject);

        return newAudioObject;
    }

    public static AudioPlayer Play(AudioClip clip, Channel channel)
    {
        AudioPlayer newAudioObject = new AudioPlayer(clip);
        newAudioObject.SetChannel(channel);

        instance.queuedAudioObject.Enqueue(newAudioObject);

        return newAudioObject;
    }

    public static List<AudioPlayer> GetAudioObjectsInChannel(Channel channel)
    {
        List<AudioPlayer> result = new List<AudioPlayer>();

        foreach(AudioPlayer audioObject in instance.livingAudioObjects)
        {
            if(audioObject.channel == channel)
                result.Add(audioObject);
        }

        return result;
    }

    public static void KillAudioObject(AudioPlayer audioObject)
    {
        instance.livingAudioObjects.Remove(audioObject);
    }

    public static SFXSource GetSource()
    {
        GameObject go = instance.sourcePool.GetFirstInactive(instance.SFXSourcePrefab);
        //GameObject.DontDestroyOnLoad(go);
        SFXSource source = go.GetComponent<SFXSource>();
        source.Reset();
        return source;
    }

    private IEnumerator PlayQueued()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            while(queuedAudioObject.Count > 0)
            {
                queuedAudioObject.Dequeue().Play();
            }
            yield return 0;
        }
    }

    public static bool Exsists()
    {
        return _instance != null;
    }
}
