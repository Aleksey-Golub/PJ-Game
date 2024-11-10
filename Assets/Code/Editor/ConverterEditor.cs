using UnityEditor;

[CustomEditor(typeof(Converter))]
public class ConverterEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void RenderCustomGizmo(Converter converter, GizmoType gizmo)
    {
        converter.DropSettings.DrawRadius(converter.transform.position);
    }
}
