using UnityEngine;

public class SFXManager : MonoBehaviour
{
    
    public static SFXManager Instance; // singleton

    [SerializeField] private AudioSource soundFXObject;


    private void Awake()
    {
        if(Instance ==null)
            Instance = this;
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawn, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
    
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLen = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLen);

    }

}
