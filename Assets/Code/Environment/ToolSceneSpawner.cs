using System.Collections.Generic;
using UnityEngine;

internal class ToolSceneSpawner : MonoBehaviour
{
    [System.Serializable]
    internal struct SpawnData
    {
        [SerializeField] internal Transform Point;
        [SerializeField] internal ToolConfig ToolConfig;
    }

    [SerializeField] private Tool _toolPrefab;
    [SerializeField] private DropSettings _dropSettings = new() { DropRadius = 1.3f, MoveAfterDropTime = 0.6f, DropStrategy = DropStrategy.RandomInsideCircle };
    [SerializeField] private List<SpawnData> _spawnDatas;

    private void Start()
    {
        var toolFactory = ToolFactory.Instance;

        foreach (var data in _spawnDatas)
        {
            var tool = toolFactory.Get(data.Point.position, Quaternion.identity);
            tool.Init(data.ToolConfig);

            var dropData = DropData.Get(data.Point.position, _dropSettings, 1, out int _);
            tool.MoveAfterDrop(dropData[0]);
        }
    }
}
