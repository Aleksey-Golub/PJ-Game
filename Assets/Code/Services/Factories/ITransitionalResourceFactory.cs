using UnityEngine;

namespace Code.Services
{
    public interface ITransitionalResourceFactory : IRecyclableFactory
    {
        TransitionalResource Get(Vector3 position, Quaternion rotation);
    }
}