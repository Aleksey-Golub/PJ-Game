using UnityEngine;

[System.Serializable]
internal class SceneSpawnerSpawnData
{
    [SerializeField] internal Transform Point;
    [SerializeField] internal ScriptableObject Config;
    [SerializeField] internal int Count = 1;
}
