using System;

namespace Code.Data
{
    [Serializable]
    public class LevelData
    {
        public string Name;
        public ResourcesDatas ResourcesDatas;
        public ToolsDatas ToolsDatas;
        public SpawnersDatas SpawnersDatas;
        public ResourceSourcesDatas ResourceSourcesDatas;
        public ResourceStoragesDatas ResourceStoragesDatas;

        public LevelData(string name)
        {
            Name = name;
            ResourcesDatas = new ResourcesDatas();
            ToolsDatas = new ToolsDatas();
            SpawnersDatas = new SpawnersDatas();
            ResourceSourcesDatas = new ResourceSourcesDatas();
            ResourceStoragesDatas = new ResourceStoragesDatas();
        }
    }
}