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
        private readonly Dictionary<string, AudioGroupData> _groups = new();
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

        public AudioGroupData GetData(string group) => _groups[group];
        public bool IsMuted(string group) => _groups[group].IsMuted;
        public float GetNormalizedVolume(string group) => _groups[group].LastNormalizedValue;

        public void SwitchMute(string group)
        {
            _groups[group].IsMuted = !_groups[group].IsMuted;

            float newNormValue = IsMuted(group) ? 0 : _groups[group].LastNormalizedValue;
            SetNormalizedVolumeInner(group, newNormValue);
        }

        public void SetNormalizedVolume(string group, float value)
        {
            _groups[group].LastNormalizedValue = value;
            SetNormalizedVolumeInner(group, value);
        }

        public void PlaySfxAtUI(AudioClip clip)
        {
            PlaySfxAtPosition(clip, Camera.main.transform.position);
        }

        public void PlaySfxAtPosition(AudioClip clip, Vector3 position)
        {
            if (!_sfxPool.TryDequeue(out AudioSource sfx))
                sfx = CreateAudioSource(_groups[SFX].AudioMixerGroup, false);

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
                _musicSource = CreateAudioSource(_groups[MUSIC].AudioMixerGroup, true);
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
                AudioSource source = CreateAudioSource(_groups[SFX].AudioMixerGroup, false);

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
            _groups.Add(SFX, new AudioGroupData(SFX, _audioMixer.FindMatchingGroups(SFX)[0], false, 1));
            _groups.Add(MUSIC, new AudioGroupData(MUSIC, _audioMixer.FindMatchingGroups(MUSIC)[0], false, 1));
        }

        private Transform CreateAudioSourceContainer()
        {
            var go = new GameObject("Audio Source Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }

        private void SetNormalizedVolumeInner(string group, float value)
        {
            value = Mathf.Max(0.0001f, value);
            // value = (0, 1]
            float newValue = Mathf.Log10(value) * 20f;
            _audioMixer.SetFloat(group, newValue);
        }
    }

    public class AudioGroupData
    {
        public string Name;
        public AudioMixerGroup AudioMixerGroup;
        public bool IsMuted;
        public float LastNormalizedValue;

        public AudioGroupData(string name, AudioMixerGroup audioMixerGroup, bool isMuted, float lastNormalizedValue)
        {
            Name = name;
            AudioMixerGroup = audioMixerGroup;
            IsMuted = isMuted;
            LastNormalizedValue = lastNormalizedValue;
        }
    }
}