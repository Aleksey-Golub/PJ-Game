using Code.UI.Services;
using System;

namespace Code.UI
{
    [Serializable]
    public class WindowMatcher
    {
        public WindowId WindowId;
        public WindowBase Template;
    }
}