using UnityEngine;

[System.Serializable]
public struct SpawnGameObjectData
{
    [GameObjectIdHolder]
    public string GameObjectId;
    [Tooltip("Used as usfull spawn position pointer. Can be null, in this case for position used 'Position'")]
    public Transform Point;
    [Tooltip("Used only when 'Point' is null. Used as spawn position pointer")]
    public Vector3 Position;
}

