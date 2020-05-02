using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioUtlity : MonoBehaviour
{
    public AudioMixerGroup sfxGroup;

    private static AudioUtlity instance;

    public static AudioUtlity Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<AudioUtlity>();
            }
            return instance;
        }
    }

    public void PlaySoundEffect(AudioClip clip, Vector3 position, float destroyTime)
    {
        GameObject newSoundEffect = new GameObject(clip.name);
        newSoundEffect.AddComponent<AudioSource>();
        newSoundEffect.GetComponent<AudioSource>().clip = clip;
        newSoundEffect.GetComponent<AudioSource>().outputAudioMixerGroup = sfxGroup;
        newSoundEffect.GetComponent<AudioSource>().Play();
        newSoundEffect.transform.position = position;
        Destroy(newSoundEffect.gameObject, destroyTime);
    }
}
