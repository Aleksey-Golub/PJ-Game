using UnityEngine;

internal class DesktopInput : IPlayerInput
{
    public float GetHorizontalAxisRaw()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalAxisRaw()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool HasMoveInput()
    {
        return
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.UpArrow)
            ;
    }
}
