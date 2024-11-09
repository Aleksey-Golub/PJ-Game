using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    [CreateAssetMenu(fileName = "new WindowsMatchers", menuName = "Configs/WindowsMatchers")]
    public class WindowsMatchers : ScriptableObject
    {
        public List<WindowMatcher> Matchers;
    }
}