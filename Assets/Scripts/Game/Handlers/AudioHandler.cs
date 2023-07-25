using UnityEngine;
using Random = UnityEngine.Random;

public class AudioHandler : MonoBehaviour
{
    private static AudioSource soundSource;
    
    private static float defaultPitch;
    private static float defaultVolume;
    
    private const float PitchValue = 0.2f;
    private const float VolumeValue = 0.05f;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        defaultPitch = soundSource.pitch;
        defaultVolume = soundSource.volume;
    }
    
    public static void PlaySound(params AudioClip[] clips) 
    {
        soundSource.clip = clips[Random.Range(0, clips.Length)];

        soundSource.volume = Random.Range(defaultVolume - VolumeValue, defaultVolume + VolumeValue);
        soundSource.pitch = Random.Range(defaultPitch - PitchValue, defaultPitch + PitchValue);

        soundSource.PlayOneShot(soundSource.clip);
    }
}
