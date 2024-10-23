using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

internal class AudioService : MonoSingleton<AudioService>
{
    [SerializeField] private AudioSource _prefab;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioClip _music;

    private const string MASTER = "Master";
    private const string MUSIC = "Music";
    private const string SFX = "sfx";

    private AudioSource _musicSource;
    private AudioMixerGroup _sfxGroup;
    private Queue<AudioSource> _sfxPool;
    private readonly List<AudioSource> _toCheckEnd = new();
    private WaitForSeconds _waitForSeconds;

    private void Start()
    {
        Construct();
        PlayAmbient(_music);
    }

    private void Construct()
    {
        _sfxGroup = _audioMixer.FindMatchingGroups(SFX)[0];

        CreatePool();

        _waitForSeconds = new WaitForSeconds(1f);
        StartCoroutine(CheckClipsEndedCoroutine());
    }

    internal void PlaySfxAtPosition(AudioClip clip, Vector3 position)
    {
        if (!_sfxPool.TryDequeue(out AudioSource sfx))
            sfx = CreateAudioSource(_sfxGroup, false);

        sfx.transform.position = position;
        sfx.clip = clip;

        _toCheckEnd.Add(sfx);
        sfx.Play();
    }

    internal void PlayAmbient(AudioClip clip)
    {
        if (!_musicSource)
        {
            _musicSource = Instantiate(_prefab, transform);
            _musicSource.name = MUSIC;
            _musicSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(MUSIC)[0];
            _musicSource.loop = true;
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

    private AudioSource CreateAudioSource(AudioMixerGroup sfxGroup, bool loop)
    {
        var source = Instantiate(_prefab, transform);
        source.outputAudioMixerGroup = sfxGroup;
        source.loop = loop;
        return source;
    }
}
