using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SimpleObjectsMatchers", menuName = "Configs/Simple Object/Simple Objects Matchers")]
public class SimpleObjectsMatchers : ScriptableObject
{
    public List<SimpleObjectMatcher> Matchers;
}