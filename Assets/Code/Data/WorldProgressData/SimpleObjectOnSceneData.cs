namespace Code.Data
{
    [System.Serializable]
    public class SimpleObjectOnSceneData
    {
        public Vector3Data Position;
        public SimpleObjectType Type;
        public bool SceneBuiltInItem;

        public SimpleObjectOnSceneData(Vector3Data position, SimpleObjectType type, bool sceneBuiltInItem)
        {
            Position = position;
            Type = type;
            SceneBuiltInItem = sceneBuiltInItem;
        }
    }
}