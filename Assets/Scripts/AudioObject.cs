using UnityEngine;

public class AudioObject : MonoBehaviourWrapper
{
    private static AudioObject _instance;

    private AudioSource _audioSource;

    public static AudioObject GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(new GameObject()).AddComponent<AudioObject>();
        }

        return _instance;
    }

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
