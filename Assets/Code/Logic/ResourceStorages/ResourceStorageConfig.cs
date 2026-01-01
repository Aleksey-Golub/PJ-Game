using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newResourceStorageConfig", menuName = "Configs/Resource Storage/Resource Storage Config")]
public class ResourceStorageConfig : ScriptableObject, IUpgradable
{
    [SerializeField] private ResourceStorageType _type;
    [SerializeField] private Sprite _sprite;
    [Tooltip("Can be null")]
    [SerializeField] private Sprite _secondarySprite;
    [SerializeField] private bool _upgradable;
    [SerializeField] private List<UpgradeStaticData> _upgradeDatas;
    [field: SerializeField] public string ID { get; private set; }

    public ResourceStorageType Type => _type;
    public Sprite Sprite => _sprite;
    public Sprite SecondarySprite => _secondarySprite;
    public bool IsUpgradable => _upgradable;
    public UpgradableType UpgradableType => UpgradableType.ResourceStorage;

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

public enum ResourceStorageType
{
    None = 0,
    CoinStorage = 1,
    GrasStorage = 2,
    WoodStorage = 3,
    StoneStorage = 4,
    EggStorage = 5,
    CarrotStorage = 6,
    WaterStorage = 7,
    Chest = 8,
    CarrotInGround = 9,
}
