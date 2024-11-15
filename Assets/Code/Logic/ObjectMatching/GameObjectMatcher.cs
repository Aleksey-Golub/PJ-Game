using System;
using UnityEngine;

[Serializable]
public class GameObjectMatcher
{
    public string GameObjectId;
    [ForceInterface(typeof(ICreatedByIdGameObject))]
    public GameObject Template;
}
