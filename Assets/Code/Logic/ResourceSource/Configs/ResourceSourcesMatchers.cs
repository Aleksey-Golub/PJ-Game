using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ResourceSourcesMatchers", menuName = "Configs/Resource Source/Resource Sources Matchers")]
public class ResourceSourcesMatchers : ScriptableObject
{
    public List<ResourceSourceMatcher> Matchers;
}