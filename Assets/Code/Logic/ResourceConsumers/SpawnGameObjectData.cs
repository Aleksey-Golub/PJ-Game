using UnityEngine;

[System.Serializable]
public struct SpawnGameObjectData
{
    [GameObjectIdHolder]
    public string GameObjectId;
    public Transform Point;
}

