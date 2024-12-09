using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Code.Infrastructure;
using Code.Data;
using System;
using Object = UnityEngine.Object;

namespace Code.Services
{
    internal class AudioService : IAudioService
    {
        private const string CONTAINER_NAME = "Audio Source Container";

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
        private CreatedAudioSource _musicSource;
        private Queue<CreatedAudioSource> _sfxPool;
        private readonly Dictionary<string, CreatedAudioSource> _toCheckEnd = new();
        private readonly List<CreatedAudioSource> _cache = new();
        private readonly WaitForSeconds _waitForSeconds;
        private Transform _audioSourceContainer;
        private bool _pause;
        private readonly ICoroutineRunner _coroutineRunner;

        internal AudioService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;

            _waitForSeconds = new WaitForSeconds(1f);
        }

        public void Load()
        {
            _audioSourceContainer = FactoryHelper.CreateDontDestroyOnLoadGameObject(CONTAINER_NAME).transform;
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

        public void PlaySfxAtUI(AudioClip clip, string objectUniqueId = default, bool looping = false)
        {
            PlaySfxAtPosition(clip, Camera.main.transform.position, objectUniqueId, looping);
        }

        public string PlaySfxAtPosition(AudioClip clip, Vector3 position, string objectUniqueId = default, bool looping = false)
        {
            if (!_sfxPool.TryDequeue(out CreatedAudioSource sfx))
                sfx = CreateAudioSource(_groups[SFX].AudioMixerGroup, false);

            sfx.AudioSource.transform.position = position;
            sfx.AudioSource.clip = clip;
            sfx.AudioSource.loop = looping;
            sfx.ObjectUniqueId = objectUniqueId;

            _toCheckEnd.Add(sfx.Id, sfx);
            sfx.AudioSource.Play();

            return sfx.Id;
        }

        public bool IsSfxPlaying(AudioClip clip, string audioSourceId, string objectUniqueId)
        {
            return FindPlaying(clip, audioSourceId, objectUniqueId) != null;
        }

        public void StopSfx(AudioClip clip, string audioSourceId, string objectUniqueId)
        {
            CreatedAudioSource s = FindPlaying(clip, audioSourceId, objectUniqueId);

            if (s != null)
                s.AudioSource.Stop();
        }

        private CreatedAudioSource FindPlaying(AudioClip clip, string audioSourceId, string objectUniqueId)
        {
            if (string.IsNullOrWhiteSpace(audioSourceId))
                return default;

            if (_toCheckEnd.TryGetValue(audioSourceId, out CreatedAudioSource s))
                if (s.ObjectUniqueId == objectUniqueId && s.AudioSource.clip == clip && s.AudioSource.isPlaying)
                    return s;

            return default;
        }

        public void PlayAmbient()
        {
            PlayAmbient(_ambient);
        }

        public void PlayAmbient(AudioClip clip)
        {
            if (_musicSource == null)
            {
                _musicSource = CreateAudioSource(_groups[MUSIC].AudioMixerGroup, true);
                _musicSource.AudioSource.name = MUSIC;
            }

            _musicSource.AudioSource.Stop();
            _musicSource.AudioSource.clip = clip;

            _musicSource.AudioSource.Play();
        }

        public void ReadAppSettings(AppSettings appSettings)
        {
            // becuse of Unity bug
            // _audioMixer.SetFloat(..) not work correct in Awake()
            _coroutineRunner.StartCoroutine(ReadAppSettingsCor(appSettings));
        }

        private IEnumerator ReadAppSettingsCor(AppSettings appSettings)
        {
            yield return null;

            if (appSettings.AudioSettings.AudioGroupSettings == null)
            {
                foreach (AudioGroupData g in _groups.Values)
                {
                    g.LastNormalizedValue = appSettings.AudioSettings.DefaultNormalizedVolume;
                    SetNormalizedVolume(g.Name, g.LastNormalizedValue);
                }

                yield break;
            }

            foreach (AudioGroupData g in _groups.Values)
            {
                AudioGroupSettings gSetting = appSettings.AudioSettings.AudioGroupSettings.Find(s => s.Name == g.Name);
                if (gSetting != null)
                {
                    g.IsMuted = gSetting.IsMuted;
                    g.LastNormalizedValue = gSetting.LastNormalizedValue;
                }
            }

            foreach (AudioGroupData g in _groups.Values)
                SetNormalizedVolumeInner(g.Name, g.IsMuted ? 0 : g.LastNormalizedValue);
        }

        public void WriteToAppSettings(AppSettings appSettings)
        {
            List<AudioGroupSettings> list = new();

            foreach (AudioGroupData g in _groups.Values)
                list.Add(new AudioGroupSettings(g.Name, g.IsMuted, g.LastNormalizedValue));

            appSettings.AudioSettings.AudioGroupSettings = list;
        }

        private IEnumerator CheckClipsEndedCoroutine()
        {
            while (true)
            {
                yield return _waitForSeconds;

                foreach (KeyValuePair<string, CreatedAudioSource> item in _toCheckEnd)
                {
                    if (!item.Value.AudioSource.isPlaying && Application.isFocused && !_pause)
                    {
                        _cache.Add(item.Value);
                    }
                }

                foreach (var s in _cache)
                {
                    _sfxPool.Enqueue(s);
                    _toCheckEnd.Remove(s.Id);
                }

                _cache.Clear();
            }
        }

        private void CreatePool()
        {
            const int capacity = 10;

            _sfxPool = new(capacity);
            for (int i = 0; i < capacity; i++)
            {
                CreatedAudioSource source = CreateAudioSource(_groups[SFX].AudioMixerGroup, false);

                _sfxPool.Enqueue(source);
            }
        }

        private CreatedAudioSource CreateAudioSource(AudioMixerGroup group, bool loop)
        {
            AudioSource source = Object.Instantiate(_prefab, _audioSourceContainer);
            source.outputAudioMixerGroup = group;
            source.loop = loop;

            return new CreatedAudioSource(source, Guid.NewGuid().ToString());
        }

        private void CacheGroups()
        {
            _groups.Add(SFX, new AudioGroupData(SFX, _audioMixer.FindMatchingGroups(SFX)[0], false, 1));
            _groups.Add(MUSIC, new AudioGroupData(MUSIC, _audioMixer.FindMatchingGroups(MUSIC)[0], false, 1));
        }

        private void SetNormalizedVolumeInner(string group, float value)
        {
            value = Mathf.Max(0.0001f, value);
            // value = (0, 1]
            float newValue = Mathf.Log10(value) * 20f;
            _audioMixer.SetFloat(group, newValue);
        }

        public void PauseAll()
        {
            Logger.Log($"[Audio] PauseAll()");
            _pause = true;

            if (_musicSource != null)
                _musicSource.AudioSource.Pause();

            foreach (var s in _toCheckEnd)
                s.Value.AudioSource.Pause();
        }

        public void UnPauseAll()
        {
            Logger.Log($"[Audio] UnPauseAll()");
            _pause = false;

            if (_musicSource != null)
                _musicSource.AudioSource.UnPause();

            foreach (var s in _toCheckEnd)
                s.Value.AudioSource.Pause();
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

    public class CreatedAudioSource
    {
        public AudioSource AudioSource { get; }
        public string Id { get; }
        public string ObjectUniqueId { get; set; }

        public CreatedAudioSource(AudioSource audioSource, string id)
        {
            AudioSource = audioSource;
            Id = id;
        }
    }
}