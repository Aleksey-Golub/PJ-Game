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
        public SimpleObjectsDatas SimpleObjectsDatas;
        public WorkbenchesDatas WorkbenchesDatas;
        public ChunksDatas ChunksDatas;
        public WorkshopsDatas WorkshopsDatas;
        public ConvertersDatas ConvertersDatas;

        public LevelData(string name)
        {
            Name = name;
            ResourcesDatas = new ResourcesDatas();
            ToolsDatas = new ToolsDatas();
            SpawnersDatas = new SpawnersDatas();
            ResourceSourcesDatas = new ResourceSourcesDatas();
            ResourceStoragesDatas = new ResourceStoragesDatas();
            SimpleObjectsDatas = new SimpleObjectsDatas();
            WorkbenchesDatas = new WorkbenchesDatas();
            ChunksDatas = new ChunksDatas();
            WorkshopsDatas = new WorkshopsDatas();
            ConvertersDatas = new ConvertersDatas();
        }
    }
}