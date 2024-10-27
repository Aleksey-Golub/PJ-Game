using UnityEngine;

namespace Code.Services
{
    internal class MobileInput : IInputService
    {
        private const string ELEMENTS_PATH = "Hud/Mobile Input Elements";
        private const float THRESHOLD = 0.3f;

        private MobileInputElements _elements;

        private Joystick Joystick => _elements.Joystick;

        public float GetHorizontalAxisRaw()
        {
            return Joystick.Horizontal > THRESHOLD ? 1 : Joystick.Horizontal < -THRESHOLD ? -1 : 0;
        }

        public float GetVerticalAxisRaw()
        {
            return Joystick.Vertical > THRESHOLD ? 1 : Joystick.Vertical < -THRESHOLD ? -1 : 0;
        }

        public bool HasMoveInput()
        {
            return
                Joystick.Horizontal > THRESHOLD
                || Joystick.Horizontal < -THRESHOLD
                || Joystick.Vertical > THRESHOLD
                || Joystick.Vertical < -THRESHOLD;
        }

        public void Init()
        {
            var prefab = Resources.Load<MobileInputElements>(ELEMENTS_PATH);
            _elements = Object.Instantiate(prefab);

            Object.DontDestroyOnLoad(_elements);
        }
    }
}