using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Code.Infrastructure;

namespace Code.Services
{
    internal class AudioService : IAudioService
    {
        private const string AMBIENT_PATH = "Sounds/music_loop";
        private const string AUDIOMIXER_PATH = "Sounds/AudioMixer";
        private const string AUDIOSOURCE_PREFAB_PATH = "Sounds/AudioSource";

        public const string MASTER = "Master";
        public const string MUSIC = "Music";
        public const string SFX = "sfx";

        private AudioClip _ambient;
        private AudioMixer _audioMixer;
        private AudioMixerGroup _sfxGroup;
        private AudioMixerGroup _musicGroup;
        private AudioSource _prefab;
        private AudioSource _musicSource;
        private Queue<AudioSource> _sfxPool;
        private readonly List<AudioSource> _toCheckEnd = new();
        private readonly WaitForSeconds _waitForSeconds;
        private Transform _audioSourceContainer;
        private readonly ICoroutineRunner _coroutineRunner;

        internal AudioService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;

            _waitForSeconds = new WaitForSeconds(1f);
        }

        public void Load()
        {

            _audioSourceContainer = CreateAudioSourceContainer();
            _audioMixer = Resources.Load<AudioMixer>(AUDIOMIXER_PATH);
            _prefab = Resources.Load<AudioSource>(AUDIOSOURCE_PREFAB_PATH);

            _ambient = Resources.Load<AudioClip>(AMBIENT_PATH);

            CacheGroups();
            CreatePool();

            _coroutineRunner.StartCoroutine(CheckClipsEndedCoroutine());
        }

        public bool IsMuted(string group)
        {
            _audioMixer.GetFloat(group, out float value);

            return value < -79f;
        }

        public void SwitchMute(string group)
        {
            float newValue = IsMuted(group) ? 0f : -80f;
            _audioMixer.SetFloat(group, newValue);
        }

        public void PlaySfxAtUI(AudioClip clip)
        {
            PlaySfxAtPosition(clip, Camera.main.transform.position);
        }

        public void PlaySfxAtPosition(AudioClip clip, Vector3 position)
        {
            if (!_sfxPool.TryDequeue(out AudioSource sfx))
                sfx = CreateAudioSource(_sfxGroup, false);

            sfx.transform.position = position;
            sfx.clip = clip;

            _toCheckEnd.Add(sfx);
            sfx.Play();
        }

        public void PlayAmbient()
        {
            PlayAmbient(_ambient);
        }

        public void PlayAmbient(AudioClip clip)
        {
            if (!_musicSource)
            {
                _musicSource = CreateAudioSource(_musicGroup, true);
                _musicSource.name = MUSIC;
            }

            _musicSource.Stop();
            _musicSource.clip = clip;

            _musicSource.Play();
        }

        private IEnumerator CheckClipsEndedCoroutine()
        {
            while (true)
            {
                yield return _waitForSeconds;

                for (int i = 0; i < _toCheckEnd.Count; i++)
                {
                    if (_toCheckEnd[i].isPlaying == false)
                    {
                        _sfxPool.Enqueue(_toCheckEnd[i]);

                        _toCheckEnd.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void CreatePool()
        {
            const int capacity = 10;

            _sfxPool = new(capacity);
            for (int i = 0; i < capacity; i++)
            {
                AudioSource source = CreateAudioSource(_sfxGroup, false);

                _sfxPool.Enqueue(source);
            }
        }

        private AudioSource CreateAudioSource(AudioMixerGroup group, bool loop)
        {
            AudioSource source = Object.Instantiate(_prefab, _audioSourceContainer);
            source.outputAudioMixerGroup = group;
            source.loop = loop;
            return source;
        }

        private void CacheGroups()
        {
            _sfxGroup = _audioMixer.FindMatchingGroups(SFX)[0];
            _musicGroup = _audioMixer.FindMatchingGroups(MUSIC)[0];
        }

        private Transform CreateAudioSourceContainer()
        {
            var go = new GameObject("Audio Source Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}