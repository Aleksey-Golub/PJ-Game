namespace Code.Data
{
    [System.Serializable]
    public class SpawnGameObjectData
    {
        public string GameObjectId;
        public Vector3Data Position;

        public SpawnGameObjectData(string gameObjectId, Vector3Data position)
        {
            GameObjectId = gameObjectId;
            Position = position;
        }
    }
}