using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAllConvertersConfigs", menuName = "Configs/Converters/Converters Configs")]
public class ConvertersConfigs : ScriptableObject
{
    public List<ConverterConfig> Configs;
}
