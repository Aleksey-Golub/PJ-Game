using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newToolConfig", menuName = "Configs/ToolConfig")]
internal class ToolConfig : ScriptableObject, IDropObjectConfig
{
    [SerializeField] private ToolType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private AudioClip _pickupAudio;
    [SerializeField] private bool _upgradable;
    [SerializeField] private List<UpgradeStaticData> _upgradeDatas;

    internal ToolType Type => _type;
    public Sprite Sprite => _sprite;
    internal AudioClip PickupAudio => _pickupAudio;
    internal bool IsUpgradable => _upgradable;
    internal IList<UpgradeStaticData> UpgradeStaticDatas => _upgradeDatas;
    internal string ID => Type.ToString();
}

[Serializable]
public struct UpgradeStaticData
{
    public float Value;
    public int Cost;
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

internal class ResourceStorageConfig : ScriptableObject
{

}

interface IUpgradable
{
    Sprite Sprite { get; }
    /// <summary>
    /// Practiscal result of upgrade
    /// </summary>
    float Value { get; }
    UpgradableType Type { get; }
    
}

public enum UpgradableType
{
    None = 0,
    Tool = 1,
    ResourceStorage = 2,
    Converter = 3,
}
