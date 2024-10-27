using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAllResourcesConfigs", menuName = "Configs/Resources/Resources Configs")]
public class ResourcesConfigs : ScriptableObject
{
    public List<ResourceConfig> Configs;
}
