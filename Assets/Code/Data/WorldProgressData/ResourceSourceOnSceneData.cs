using System;

namespace Code.Data
{
    [Serializable]
    public class ResourceSourceOnSceneData
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public ResourceSourceType Type;
        public ResourceType DropResourceType;
        public int DropResourceCount;
        public float RestorationTimer;
        public int CurrentHitPoints;

        public ResourceSourceOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceSourceType type,
            ResourceType dropResourceType,
            int dropResourceCount,
            float restorationTimer,
            int currentHitPoints
            )
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            Type = type;
            DropResourceType = dropResourceType;
            DropResourceCount = dropResourceCount;
            RestorationTimer = restorationTimer;
            CurrentHitPoints = currentHitPoints;
        }
    }
}