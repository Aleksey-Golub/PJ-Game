namespace Code.Data
{
    [System.Serializable]
    public class PositionOnLevel
    {
        public string Level;

        public PositionOnLevel(string initialLevel)
        {
            Level = initialLevel;
        }
    }
}