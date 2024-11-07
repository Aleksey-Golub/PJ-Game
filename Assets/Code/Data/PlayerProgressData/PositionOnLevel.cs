namespace Code.Data
{
    [System.Serializable]
    public class PositionOnLevel
    {
        public string Level;
        public Vector3Data Position;
        public Vector2Data Direction;

        public PositionOnLevel(string initialLevel)
        {
            Level = initialLevel;
        }

        public PositionOnLevel(string level, Vector3Data position, Vector2Data direction)
        {
            Level = level;
            Position = position;
            Direction = direction;
        }
    }
}