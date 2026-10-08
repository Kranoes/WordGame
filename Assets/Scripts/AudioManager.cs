using UnityEngine;

namespace GuessWordGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSfxSource;

        [Header("Default UI Sounds")]
        [SerializeField] private AudioClip defaultButtonClickSound;

        public AudioSource MusicSource => musicSource;
        public AudioSource SfxSource => sfxSource;
        public AudioSource UiSfxSource => uiSfxSource;

        [Header("Game SFX Clips")]
        public AudioClip keyTapClip;
        public AudioClip wordErrorClip;
        public AudioClip winClip;
        public AudioClip loseClip;

        public AudioClip DefaultButtonClickSound => defaultButtonClickSound;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplyAllVolumes();
        }

        public void ApplyAllVolumes()
        {
            GameData data = SaveManager.CurrentData;
            if (data == null) return;

            SetMusicVolume(data.musicVolume);
            SetSfxVolume(data.sfxVolume);
            SetUiSfxVolume(data.uiSfxVolume);
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null) musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void SetUiSfxVolume(float volume)
        {
            if (uiSfxSource != null) uiSfxSource.volume = Mathf.Clamp01(volume);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;

            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayUiSound()
        {
            PlayUiSound(defaultButtonClickSound);
        }

        public void PlayUiSound(AudioClip clip)
        {
            AudioClip clipToPlay = clip != null ? clip : defaultButtonClickSound;
            if (clipToPlay != null && uiSfxSource != null)
            {
                uiSfxSource.PlayOneShot(clipToPlay);
            }
        }
    }
}