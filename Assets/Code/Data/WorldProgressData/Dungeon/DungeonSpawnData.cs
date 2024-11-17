namespace Code.Data
{
    [System.Serializable]
    public class DungeonSpawnData
    {
        public string ResourceSourceGameObjectId;
        public Vector3Data[] LocalPositions;

        public DungeonSpawnData(string resourceSourceGameObjectId, Vector3Data[] localPositions)
        {
            ResourceSourceGameObjectId = resourceSourceGameObjectId;
            LocalPositions = localPositions;
        }
    }
}