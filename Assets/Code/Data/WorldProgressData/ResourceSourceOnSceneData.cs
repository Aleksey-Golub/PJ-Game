using System;

namespace Code.Data
{
    [Serializable]
    public class ResourceSourceOnSceneData
    {
        public Vector3Data Position;
        public ResourceSourceType Type;
        public ResourceType DropResourceType;
        public int DropResourceCount;
        public float RestorationTimer;
        public int CurrentHitPoints;
        public bool SceneBuiltInItem;

        public ResourceSourceOnSceneData(
            Vector3Data position,
            ResourceSourceType type,
            ResourceType dropResourceType,
            int dropResourceCount,
            float restorationTimer,
            int currentHitPoints,
            bool sceneBuiltInItem)
        {
            Position = position;
            Type = type;
            DropResourceType = dropResourceType;
            DropResourceCount = dropResourceCount;
            RestorationTimer = restorationTimer;
            CurrentHitPoints = currentHitPoints;
            SceneBuiltInItem = sceneBuiltInItem;
        }
    }
}