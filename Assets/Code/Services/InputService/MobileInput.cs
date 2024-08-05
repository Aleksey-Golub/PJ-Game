internal class MobileInput : IPlayerInput
{
    private const float THRESHOLD = 0.3f;

    private readonly Joystick _joystick;

    public float XMovement => _joystick.Horizontal;
    public float YMovement => _joystick.Vertical;

    public MobileInput(Joystick joystick)
    {
        _joystick = joystick;
        _joystick.gameObject.SetActive(true);
    }

    public float GetHorizontalAxisRaw()
    {
        return _joystick.Horizontal > THRESHOLD ? 1 : _joystick.Horizontal < -THRESHOLD ? -1 : 0;
    }

    public float GetVerticalAxisRaw()
    {
        return _joystick.Vertical > THRESHOLD ? 1 : _joystick.Vertical < -THRESHOLD ? -1 : 0;
    }

    public bool HasMoveInput()
    {
        return 
            _joystick.Horizontal > THRESHOLD 
            || _joystick.Horizontal < -THRESHOLD 
            || _joystick.Vertical > THRESHOLD
            || _joystick.Vertical < -THRESHOLD;
    }
}
