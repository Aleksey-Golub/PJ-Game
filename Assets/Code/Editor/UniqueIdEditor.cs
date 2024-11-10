using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Code.Editor
{
    [CustomEditor(typeof(UniqueId))]
    public class UniqueIdEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            UniqueId uniqueId = (UniqueId)target;

            if (IsPrefab(uniqueId))
                return;

            if (IsEditingInOwnScene(uniqueId))
                return;

            if (string.IsNullOrEmpty(uniqueId.Id))
                Generate(uniqueId);
            else
            {
                //Debug.Log($"{uniqueId.gameObject.name} FindObjectsOfType() called");
                UniqueId[] uniqueIds = FindObjectsOfType<UniqueId>();

                if (uniqueIds.Any(other => other != uniqueId && other.Id == uniqueId.Id))
                    Generate(uniqueId);
            }
        }

        private bool IsPrefab(UniqueId uniqueId) =>
          uniqueId.gameObject.scene.rootCount == 0;

        private bool IsEditingInOwnScene(UniqueId uniqueId) =>
            uniqueId.gameObject.scene.name == uniqueId.gameObject.name;

        private void Generate(UniqueId uniqueId)
        {
            uniqueId.GenerateId();

            if (!Application.isPlaying)
            {
                //Debug.Log($"{uniqueId.gameObject.name} Generate called");
                Undo.RecordObject(target, "Generated Id");
                //EditorUtility.SetDirty(uniqueId);
                //EditorSceneManager.MarkSceneDirty(uniqueId.gameObject.scene);
            }
        }
    }
}