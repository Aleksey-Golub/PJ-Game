using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ObjectsMatchers", menuName = "Configs/Objects Matchers")]
public class ObjectsMatchers : ScriptableObject
{
    public List<GameObjectMatcher> Configs;
}
