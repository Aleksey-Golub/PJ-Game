using System;

namespace Assets.Code.Data
{
    [Serializable]
    public class GameProgress
    {
        public PlayerProgress PlayerProgress;

        public GameProgress()
        {
            PlayerProgress = new PlayerProgress();
        }
    }
}