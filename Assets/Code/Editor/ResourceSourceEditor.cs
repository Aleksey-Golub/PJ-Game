using UnityEditor;

[CustomEditor(typeof(ResourceSource))]
public class ResourceSourceEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void RenderCustomGizmo(ResourceSource resourceSource, GizmoType gizmo)
    {
        resourceSource.DropSettings.DrawRadius(resourceSource.transform.position);
    }
}