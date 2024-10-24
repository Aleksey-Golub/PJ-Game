using UnityEngine;

interface IUpgradable
{
    Sprite Sprite { get; }
    bool IsUpgradable { get; }
    UpgradableType UpgradableType { get; }
    string ID { get; }
    int GetMaxLevel();
    UpgradeStaticData GetUpgradeData(int level);
}

public enum UpgradableType
{
    None = 0,
    Tool = 1,
    ResourceStorage = 2,
    Converter = 3,
}
