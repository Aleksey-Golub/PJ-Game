using UnityEngine;

[CreateAssetMenu(fileName = "newToolConfig", menuName = "Configs/ToolConfig")]
internal class ToolConfig : ScriptableObject, IDropObjectConfig
{
    [SerializeField] private ToolType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private AudioClip _pickupAudio;

    internal ToolType Type => _type;
    public Sprite Sprite => _sprite;
    internal AudioClip PickupAudio => _pickupAudio;
}

internal enum ToolType
{
    None    = 0,
    SICKLE  = 1,
    AXE     = 2,
    PICKAXE = 3,
    SWORD   = 4,
    BUCKET  = 5,
}
