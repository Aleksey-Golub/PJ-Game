using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSkinsConfigs", menuName = "Configs/Skins/Skins Configs")]
public class SkinsConfigs : ScriptableObject
{
    public List<SkinConfig> Configs;
}