using System;

namespace Code.Data
{
    [Serializable]
    public class AudioGroupSettings
    {
        public string Name;
        public bool IsMuted;
        public float LastNormalizedValue;

        public AudioGroupSettings(string name, bool isMuted, float lastNormalizedValue)
        {
            Name = name;
            IsMuted = isMuted;
            LastNormalizedValue = lastNormalizedValue;
        }
    }
}