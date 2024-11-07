using UnityEngine;

namespace Code.Services
{
    public interface ITransitionalResourceFactory : IRecyclableFactory
    {
        void Load();
        TransitionalResource Get(Vector3 position, Quaternion rotation);
        void Cleanup();
    }
}