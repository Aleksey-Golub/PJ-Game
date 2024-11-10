using UnityEditor;

[CustomEditor(typeof(Workbench))]
public class WorkbenchEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void RenderCustomGizmo(Workbench workbench, GizmoType gizmo)
    {
        workbench.DropSettings.DrawRadius(workbench.transform.position);
    }
}
