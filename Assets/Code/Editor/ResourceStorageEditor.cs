using UnityEditor;

[CustomEditor(typeof(ResourceStorage))]
public class ResourceStorageEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void RenderCustomGizmo(ResourceStorage resourceStorage, GizmoType gizmo)
    {
        resourceStorage.DropSettings.DrawRadius(resourceStorage.transform.position);
    }
}