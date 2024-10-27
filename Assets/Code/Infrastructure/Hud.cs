using Code.UI;
using UnityEngine;

namespace Code.Infrastructure
{
    public class Hud : MonoBehaviour
    {
        [field: SerializeField] internal PlayerInventoryView PlayerInventoryView { get; private set; }
    }
}