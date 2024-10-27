using Code.UI.Services;
using System;

namespace Code.UI
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId WindowId;
        public WindowBase Template;
    }
}