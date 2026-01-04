using System.Collections.Generic;

namespace Code.Data
{
    [System.Serializable]
    public class ConverterOnSceneData
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public bool IsAvailable;
        public ConverterType Type;
        public int CurrentUpload;
        public List<UploadData> CurrentUploads = new();
        public float Timer;

        public ConverterOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            bool isAvailable,
            ConverterType type,
            List<UploadData> currentUploads,
            float timer
            )
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            IsAvailable = isAvailable;
            Type = type;
            CurrentUploads = currentUploads;
            Timer = timer;
        }
    }

    [System.Serializable]
    public class UploadData
    {
        public ResourceType ResourceType;
        public int CurrentUpload;

        public UploadData(ResourceType resourceType, int currentUpload)
        {
            ResourceType = resourceType;
            CurrentUpload = currentUpload;
        }
    }
}