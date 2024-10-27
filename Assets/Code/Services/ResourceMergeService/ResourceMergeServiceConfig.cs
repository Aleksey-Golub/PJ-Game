using UnityEngine;

[CreateAssetMenu(fileName = "newResourceMergeServiceConfig", menuName = "Configs/ResourceMergeServiceConfig")]
public class ResourceMergeServiceConfig : ScriptableObject
{
    [field: SerializeField] public float DistanceToMerge = 1.2f;
    [field: SerializeField] public float TimeToMerge = 1f;
}
