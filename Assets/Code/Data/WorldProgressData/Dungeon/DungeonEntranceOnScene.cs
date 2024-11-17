namespace Code.Data
{
    [System.Serializable]
    public class DungeonEntranceOnScene
    {
        public bool IsForcedOpen;
        public TimerData TimerData;

        public DungeonEntranceOnScene(
            bool isForcedOpen,
            TimerData timerData
            )
        {
            IsForcedOpen = isForcedOpen;
            TimerData = timerData;
        }
    }
}