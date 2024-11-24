using System;

namespace Code.Data
{
    [Serializable]
    public class TutorialProgress
    {
        public bool IsCompleted;
        public int Stage;
        public int[] GoalsProgresses;
        public TutorialObjectsDatas TutorialObjectsDatas;

        public TutorialProgress()
        {
            TutorialObjectsDatas = new TutorialObjectsDatas();
        }
    }
}