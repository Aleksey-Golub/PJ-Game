using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class GameObjectIdHolderAttribute : PropertyAttribute
{
    public GameObjectType GameObjectType { get; private set; }

    public GameObjectIdHolderAttribute(GameObjectType gameObjectType = GameObjectType.All)
    {
        GameObjectType = gameObjectType;
    }
}

[Flags]
public enum GameObjectType
{
    None = 0,
    SimpleObject = 1,
    ResourceSource = 2,
    ResourceStorage = 4,
    ResourceConsumer = 8,
    Dungeon = 16,
    Special = 32,
    All = ~0,
}
