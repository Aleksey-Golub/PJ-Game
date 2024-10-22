using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newToolConfig", menuName = "Configs/ToolConfig")]
internal class ToolConfig : ScriptableObject, IDropObjectConfig, IUpgradable
{
    [SerializeField] private ToolType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private AudioClip _pickupAudio;
    [SerializeField] private bool _upgradable;
    [SerializeField] private List<UpgradeStaticData> _upgradeDatas;

    public Sprite Sprite => _sprite;
    public bool IsUpgradable => _upgradable;
    public UpgradableType UpgradableType => UpgradableType.Tool;
    public string ID => Type.ToString();
    internal ToolType Type => _type;
    internal AudioClip PickupAudio => _pickupAudio;

    public UpgradeStaticData GetUpgradeData(int level)
    {
        int levelIndex = level - 1;
        if (levelIndex < 0 || levelIndex >= _upgradeDatas.Count)
        {
            Logger.LogError($"[ToolConfig] for {_type} Error: level {level} is not implemented. Return default");
            return default;
        }
        
        return _upgradeDatas[levelIndex];
    }

    public int GetMaxLevel() => _upgradeDatas.Count;
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
