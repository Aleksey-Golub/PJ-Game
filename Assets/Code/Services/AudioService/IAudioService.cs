using UnityEngine;

namespace Code.Services
{
    public interface IAudioService : IService
    {
        bool IsMuted(string group);
        void Load();
        void PlayAmbient();
        void PlayAmbient(AudioClip clip);
        void PlaySfxAtPosition(AudioClip clip, Vector3 position);
        void PlaySfxAtUI(AudioClip clip);
        void SwitchMute(string group);
    }
}