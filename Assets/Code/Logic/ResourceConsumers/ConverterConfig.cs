using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newConverterConfig", menuName = "Configs/ConverterConfig")]
internal class ConverterConfig : ScriptableObject, IUpgradable
{
    [SerializeField] private ConverterType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private bool _upgradable;
    [SerializeField] private List<UpgradeStaticData> _upgradeDatas;

    public ConverterType Type => _type;
    public Sprite Sprite => _sprite;
    public bool IsUpgradable => _upgradable;
    public UpgradableType UpgradableType => UpgradableType.ResourceStorage;
    public string ID => _type.ToString();

    public UpgradeStaticData GetUpgradeData(int level)
    {
        int levelIndex = level - 1;
        if (levelIndex < 0 || levelIndex >= _upgradeDatas.Count)
        {
            Logger.LogError($"[ConverterConfig] for {ID} Error: level {level} is not implemented. Return default");
            return default;
        }

        return _upgradeDatas[levelIndex];
    }

    public int GetMaxLevel() => _upgradeDatas.Count;
}

internal enum ConverterType
{
    None = 0,
    CowConverter = 1,
    PigConverter = 2,
    FurnaceConverter = 3,
}
