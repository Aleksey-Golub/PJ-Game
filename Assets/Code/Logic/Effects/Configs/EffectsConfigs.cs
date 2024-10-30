using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EffectsConfigs", menuName = "Configs/EffectsConfigs")]
public class EffectsConfigs : ScriptableObject
{
    public List<EffectConfig> Configs;
}
