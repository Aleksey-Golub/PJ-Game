using System;
using System.Collections.Generic;

namespace Code.Data
{
    [Serializable]
    public class AudioSettings
    {
        public float DefaultNormalizedVolume = 1.0f;
        public List<AudioGroupSettings> AudioGroupSettings = null;
    }
}