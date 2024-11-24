namespace Code.Data
{
    [System.Serializable]
    public class TutorialObjectOnSceneData : SimpleObjectOnSceneData
    {
        public string GameObjectId;

        public TutorialObjectOnSceneData(
            Vector3Data position,
            SimpleObjectType type,
            bool sceneBuiltInItem,
            string gameObjectId
            ) : base(position, type, sceneBuiltInItem)
        {
            GameObjectId = gameObjectId;
        }
    }
}