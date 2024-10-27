using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAllToolsConfigs", menuName = "Configs/Tools/Tools Configs")]
public class ToolsConfigs : ScriptableObject
{
    public List<ToolConfig> Configs;
}