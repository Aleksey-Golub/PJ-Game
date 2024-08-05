using UnityEngine;

internal class InputServiceProvider : MonoSingleton<InputServiceProvider>
{
#if UNITY_EDITOR
    internal enum InputType { Desktop, Mobile }
    [SerializeField] private InputType _inputType; 
#endif

    [SerializeField] private Joystick _joystick;

    internal IPlayerInput GetService()
    {
#if UNITY_EDITOR
        return _inputType switch
        {
            InputType.Desktop => new DesktopInput(),
            InputType.Mobile => new MobileInput(_joystick),
            _ => throw new System.NotImplementedException($"[InputServiceProvider] not implemented for {_inputType}"),
        };
#else

        if (Application.platform is RuntimePlatform.Android or RuntimePlatform.IPhonePlayer)
        {
            return new MobileInput(_joystick);
        }
        else
        {
            return new DesktopInput();
        }
#endif
    }
}
