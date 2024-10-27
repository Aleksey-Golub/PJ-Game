using UnityEngine;

namespace Code.Services
{
    public interface IPopupFactory : IRecyclableFactory
    {
        Popup Get(Vector3 position, Quaternion rotation);
    }
}