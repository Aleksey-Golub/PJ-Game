using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

internal abstract class SceneSpawnerBase<T> : SceneSpawnerBase, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder where T : ScriptableObject
{
    [field: SerializeField] public UniqueId UniqueId { get; private set; }

    private bool _exhausted;
    private string Id => UniqueId.Id;

    private void OnValidate()
    {
        foreach (SpawnData d in _spawnDatas)
        {
            if (d.Config is not null && d.Config is not T)
            {
                d.Config = null;
                Debug.LogError($"Wrong config used. Set {nameof(T)}");
            }
        }
    }

    public void WriteToProgress(GameProgress progress)
    {
        List<string> exhaustedSpawners = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].SpawnersDatas.Exhausted;

        if (_exhausted && !exhaustedSpawners.Contains(Id))
            exhaustedSpawners.Add(Id);
    }

    public void ReadProgress(GameProgress progress)
    {
        if (progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].SpawnersDatas.Exhausted.Contains(Id))
            _exhausted = true;
        else
            Spawn();
    }

    protected abstract void SpawnInner();

    private void Spawn()
    {
        _exhausted = true;
        SpawnInner();
    }
}

internal abstract class SceneSpawnerBase : MonoBehaviour
{
    [SerializeField] protected DropSettings _dropSettings = DropSettings.Default;
    [SerializeField] protected List<SpawnData> _spawnDatas;

#if UNITY_EDITOR
    [CustomEditor(typeof(SceneSpawnerBase))]
    public class SceneSpawnerBaseEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void RenderCustomGizmo(SceneSpawnerBase sceneSpawner, GizmoType gizmo)
        {
            foreach (var data in sceneSpawner._spawnDatas)
                sceneSpawner._dropSettings.DrawRadius(data.Point.transform.position);
        }
    }
#endif
}