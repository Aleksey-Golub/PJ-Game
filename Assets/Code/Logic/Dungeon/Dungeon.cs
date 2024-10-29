using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private SpawnData[] _spawnDatas;
    [SerializeField] private DungeonEntrance _entrance;
    [SerializeField] private DungeonEntrance _exit;

    private List<ResourceSource> _list;

    private void Start()
    {
        _list = new List<ResourceSource>(_spawnDatas.Length);

        _exit.InteractedByPlayer += OnPlayerExit;

        Spawn();
    }

    private void OnDestroy()
    {
        _exit.InteractedByPlayer -= OnPlayerExit;
    }

    private void Spawn()
    {
        foreach (SpawnData data in _spawnDatas)
        {
            foreach (Transform p in data.Points)
            {
                var r = Instantiate<ResourceSource>(data.Prefab, p.position, Quaternion.identity, this.transform);
                r.Dropped += ResourceSourceDropped;

                _list.Add(r);
            }
        }
    }

    private void OnPlayerExit(DungeonEntrance exit)
    {
        _entrance.ReStart();
        Spawn();
        _exit.ForceClose();
    }

    private void ResourceSourceDropped(ResourceSource r)
    {
        r.Dropped -= ResourceSourceDropped;
        _list.Remove(r);

        Destroy(r.gameObject);

        if (_list.Count == 0)
            OpenExit();
    }

    private void OpenExit()
    {
        _exit.ForceOpen();
    }

    [System.Serializable]
    internal struct SpawnData
    {
        public Transform[] Points;
        public ResourceSource Prefab;
    }
}
