using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newResourceStorageConfig", menuName = "Configs/ResourceStorageConfig")]
internal class ResourceStorageConfig : ScriptableObject, IUpgradable
{
    [SerializeField] private ResourceStorageType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private bool _upgradable;
    [SerializeField] private List<UpgradeStaticData> _upgradeDatas;

    public ResourceStorageType Type => _type;
    public Sprite Sprite => _sprite;
    public bool IsUpgradable => _upgradable;
    public UpgradableType UpgradableType => UpgradableType.ResourceStorage;
    public string ID => _type.ToString();

    public UpgradeStaticData GetUpgradeData(int level)
    {
        int levelIndex = level - 1;
        if (levelIndex < 0 || levelIndex >= _upgradeDatas.Count)
        {
            Logger.LogError($"[ResourceStorageConfig] for {ID} Error: level {level} is not implemented. Return default");
            return default;
        }

        return _upgradeDatas[levelIndex];
    }

    public int GetMaxLevel() => _upgradeDatas.Count;
}

internal enum ResourceStorageType
{
    None = 0,
    CoinStorage = 1,
    GrasStorage = 2,
    WoodStorage = 3,
    StoneStorage = 4,
    EggStorage = 5,
    CarrotStorage = 6,
    WaterStorage = 7,
}