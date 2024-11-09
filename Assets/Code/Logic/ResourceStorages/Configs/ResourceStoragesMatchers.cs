using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ResourceStoragesMatchers", menuName = "Configs/Resource Storage/Resource Storages Matchers")]
public class ResourceStoragesMatchers : ScriptableObject
{
    public List<ResourceStorageMatcher> Matchers;
}