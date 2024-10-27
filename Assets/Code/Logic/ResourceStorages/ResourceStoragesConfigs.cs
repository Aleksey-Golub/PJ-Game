using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAllResourceStoragesConfigs", menuName = "Configs/Resource Storage/Resource Storages Configs")]
public class ResourceStoragesConfigs : ScriptableObject
{
    public List<ResourceStorageConfig> Configs;
}