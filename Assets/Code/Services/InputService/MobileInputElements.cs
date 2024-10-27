using UnityEngine;

namespace Code.Services
{
    public class MobileInputElements : MonoBehaviour
    {
        [field: SerializeField] public Joystick Joystick { get; private set; }
    }
}