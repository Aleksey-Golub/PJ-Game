using System.Collections.Generic;
using UnityEngine;

internal class SceneSpawnersContainer : MonoBehaviour
{
    [field: SerializeField] public List<SceneSpawnerBase> SceneSpawners { get; private set; }
}
