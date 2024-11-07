using UnityEngine;

namespace Code.Services
{
    public interface IPopupFactory : IRecyclableFactory
    {
        void Load();
        Popup Get(Vector3 position, Quaternion rotation);
        void Cleanup();
    }
}