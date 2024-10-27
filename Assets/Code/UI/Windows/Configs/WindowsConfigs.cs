using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    [CreateAssetMenu(fileName = "new WindowsConfigs", menuName = "Configs/WindowsConfigs")]
    public class WindowsConfigs : ScriptableObject
    {
        public List<WindowConfig> Configs;
    }
}