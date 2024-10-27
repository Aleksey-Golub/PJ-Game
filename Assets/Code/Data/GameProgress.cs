using System;

namespace Code.Data
{
    [Serializable]
    public class GameProgress
    {
        public PlayerProgress PlayerProgress;

        public GameProgress(string initialLevel)
        {
            PlayerProgress = new PlayerProgress(initialLevel);
        }
    }
}