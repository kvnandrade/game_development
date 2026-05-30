using System.Collections.Generic;
using UnityEngine;

namespace ValeDosCristais
{
    public sealed class AudioManager : MonoBehaviour
    {
        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private AudioSource sfxSource;
        private AudioSource musicSource;

        private void Awake()
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = 0.9f;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = 0.28f;
        }

        private void Start()
        {
            PlayMusic("music_loop");
        }

        public void Play(string clipName)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayMusic(string clipName)
        {
            AudioClip clip = GetClip(clipName);
            if (clip == null || musicSource.clip == clip)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.Play();
        }

        private AudioClip GetClip(string clipName)
        {
            AudioClip clip;
            if (clips.TryGetValue(clipName, out clip))
            {
                return clip;
            }

            clip = Resources.Load<AudioClip>("Audio/" + clipName);
            clips[clipName] = clip;
            return clip;
        }
    }
}
