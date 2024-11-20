using UnityEngine;

namespace Code.Services
{
    public interface IAudioService : IService, ISavedAppSettingsReader, ISavedAppSettingsWriter
    {
        bool IsMuted(string group);
        void Load();
        void PlayAmbient();
        void PlayAmbient(AudioClip clip);
        string PlaySfxAtPosition(AudioClip clip, Vector3 position, string objectUniqueId = default, bool looping = false);
        void PlaySfxAtUI(AudioClip clip, string objectUniqueId = default, bool looping = false);
        bool IsSfxPlaying(AudioClip clip, string audioSourceId, string objectUniqueId);
        void StopSfx(AudioClip clip, string audioSourceId, string objectUniqueId);
        void SwitchMute(string group);
        void SetNormalizedVolume(string group, float value);
        float GetNormalizedVolume(string group);
        AudioGroupData GetData(string group);
    }
}