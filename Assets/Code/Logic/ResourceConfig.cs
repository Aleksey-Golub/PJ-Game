﻿using UnityEngine;

[CreateAssetMenu(fileName = "newResourceConfig", menuName = "Configs/Resources/Resource Config")]
public class ResourceConfig : ScriptableObject, IDropObjectConfig
{
    [SerializeField] private ResourceType _type;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private AudioClip _pickupAudio;
    [SerializeField] private int _cost;
    [SerializeField] private bool _sellable;

    internal ResourceType Type => _type;
    public Sprite Sprite => _sprite;
    internal AudioClip PickupAudio => _pickupAudio;
    internal int Cost => _cost;
    public bool Sellable => _sellable;
}

public enum ResourceType
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
