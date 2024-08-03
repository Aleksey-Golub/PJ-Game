﻿using UnityEngine;

[CreateAssetMenu(fileName = "newResourceConfig", menuName = "Configs/ResourceConfig")]
internal class ResourceConfig : ScriptableObject
{
    [SerializeField] private ResourceType _type;
    [SerializeField] private Sprite _sprite;

    internal ResourceType Type => _type;
    internal Sprite Sprite => _sprite;
}

internal enum ResourceType
{
    None = 0,
    COIN = 1,
    GEM = 2,
    GRASS = 3,
    WOOD = 4,
    STONE = 5,
    MILK = 6,
    SLIME_EGG = 7,
    CARROT = 8,
    DUNG = 9,
    WATER = 10,
    FRUIT = 11,
    IRON_ORE = 12,
    IRON = 13
}
