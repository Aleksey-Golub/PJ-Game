using System;

namespace Code.Data
{
    [Serializable]
    public class GameProgress
    {
        public PlayerProgress PlayerProgress;
        public WorldProgress WorldProgress;
        public string SaveTime;

        public GameProgress(string initialLevel)
        {
            PlayerProgress = new PlayerProgress(initialLevel);
            WorldProgress = new WorldProgress();
        }
    }
}