namespace Code.Data
{
    [System.Serializable]
    public class TimerData
    {
        public bool IsStarted;
        public float Passed;
        public float Duration;

        public TimerData(
            bool isStarted,
            float passed,
            float duration)
        {
            IsStarted = isStarted;
            Passed = passed;
            Duration = duration;
        }
    }
}