namespace Code.Data
{
    [System.Serializable]
    public class ConverterOnSceneData
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public ConverterType Type;
        public int CurrentUpload;
        public float Timer;

        public ConverterOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ConverterType type,
            int currentUpload,
            float timer
            )
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            Type = type;
            CurrentUpload = currentUpload;
            Timer = timer;
        }
    }
}