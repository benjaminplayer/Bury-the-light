using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    public static SFXManager Instance; // singleton

    [SerializeField] private AudioSource soundFXObject;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawn, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        activeAudioSources.Add(audioSource);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLen = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLen);
        StartCoroutine(RemoveFromListAfterTime(audioSource, 1f));
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawn, float volume, bool loop)
    {
        if (!loop)
        { 
            PlaySFXClip(audioClip, spawn, volume);
            return;
        } 
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        activeAudioSources.Add(audioSource);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();

    }

    private IEnumerator RemoveFromListAfterTime(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeAudioSources.Remove(source);
    }

    public void StopAllAudio()
    {
        foreach (AudioSource source in activeAudioSources)
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
        }
        activeAudioSources.Clear();
    }

}
