using System;

namespace Code.Data
{
    [Serializable]
    public class ToolOnSceneData
    {
        public Vector3Data Position;
        public ToolType Type;

        public ToolOnSceneData(Vector3Data position, ToolType type)
        {
            Position = position;
            Type = type;
        }
    }
}