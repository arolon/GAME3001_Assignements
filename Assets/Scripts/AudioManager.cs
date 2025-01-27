using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] public AudioSource backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        // Play a one-shot sound effect
        backgroundMusic.PlayOneShot(clip);
    }
}
