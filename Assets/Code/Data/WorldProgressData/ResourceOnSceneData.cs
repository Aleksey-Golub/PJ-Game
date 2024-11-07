using System;

namespace Code.Data
{
    [Serializable]
    public class ResourceOnSceneData
    {
        public Vector3Data Position;
        public int Count;
        public ResourceType Type;

        public ResourceOnSceneData(Vector3Data position, int count, ResourceType type)
        {
            Position = position;
            Count = count;
            Type = type;
        }
    }
}