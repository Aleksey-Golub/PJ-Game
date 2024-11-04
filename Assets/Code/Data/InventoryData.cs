using System.Collections.Generic;

namespace Code.Data
{
    [System.Serializable]
    public class InventoryData
    {
        public List<ToolType> Tools = new();
        public ResourceStorageDataDictionary ResourceStorageData = new();
    }
}