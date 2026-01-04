using UnityEngine;

[System.Serializable]
public class DropResourceSettings
{
    [field: SerializeField] public ResourceConfig DropResourceConfig { get; private set; }
    [field: SerializeField] public int DropCount { get; private set; } = 1;
}
