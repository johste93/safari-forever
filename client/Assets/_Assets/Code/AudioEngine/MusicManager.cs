using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    private static Music currentTrack;

    private const float crossfadeDelay = 1f;
    private static bool playingNormalVersion;

    private static AudioSource currentSource;
    private static Tween[] crossfadeTweens = new Tween[2];

    public AudioSource[] sources;

    private static int nextSourceIndex = 0;
    
    private const string RESOURCE_PATH = "";
	private const string FILENAME = "MusicManager";

    private static string path { get { return RESOURCE_PATH + FILENAME; } }
    
    private static MusicManager _instance;
    private static MusicManager instance
    {
        get{
            if(_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameObject>(path)).GetComponent<MusicManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            
            return _instance;
        }
    }

    public static void Play(Music music, bool crossfade = false, bool syncPositions = false)
    {
        bool playNormalVersion = GameMaster.instance == null || GameMaster.instance.IsPlaying();
        if(music != Music.None)
        {
            if(music == currentTrack)
            {
                if(playNormalVersion == playingNormalVersion)
                {
                    return;
                }
            }
        }
        playingNormalVersion = playNormalVersion;

        bool maySyncPositions = syncPositions && currentTrack != Music.None;
        float normalizedPos = 0f;
        float currentTime = 0f;
        if(maySyncPositions)
        {
            currentTime = currentSource.time;
            normalizedPos = currentTime / currentSource.clip.length;
        }
        
        currentTrack = music;

        if(currentSource != null)
        {
            KillAllTweens();
            crossfadeTweens[nextSourceIndex] = currentSource.DOFade(0f, crossfade ? crossfadeDelay : 0f).SetEase(Ease.InQuad);
        }
            
        if(currentTrack == Music.None)
        {
            if(currentSource != null)
            {
                currentSource.volume = 0;
                currentSource = null;
            }
            
            if(!crossfade)
                KillAllTweens();

            return;
        }

        Track track = MusicCollection.tracks.Get(music);
        AudioClip clip = playNormalVersion ? track.normal : track.soft;

        float startPos = maySyncPositions ? normalizedPos * clip.length : 0f;

        nextSourceIndex++;
        if(nextSourceIndex >= instance.sources.Length)
            nextSourceIndex = 0;

        AudioSource nextSource = instance.sources[nextSourceIndex];
        nextSource.clip = clip;
        nextSource.volume = crossfade ? 0f : Globals.musicConstants.defaultVolume;
        nextSource.time = startPos;
        nextSource.Play();
        nextSource.mute = !SaveManager.currentSave.music;
        currentSource = nextSource;

        if(crossfade)
        {
            crossfadeTweens[nextSourceIndex] = nextSource.DOFade(Globals.musicConstants.defaultVolume, crossfadeDelay).SetEase(Ease.OutQuad);
        }
    }

    public static void On_MuteMusicButtonClicked()
    {
        foreach(AudioSource aS in instance.sources)
            aS.mute = !SaveManager.currentSave.music;
    }

    public static void DoFade(float endValue, float duration)
    {
        if(currentSource == null)
            return;
            
        currentSource.DOKill();
        currentSource.DOFade(endValue, duration);
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private static void KillAllTweens()
	{
		crossfadeTweens[0]?.Kill();
        crossfadeTweens[1]?.Kill();

        crossfadeTweens[0] = null;
        crossfadeTweens[1] = null;
	}
}
