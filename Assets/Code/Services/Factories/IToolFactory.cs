using UnityEngine;

namespace Code.Services
{
    public interface IToolFactory : IRecyclableFactory
    {
        void Load();
        Tool Get(Vector3 position, Quaternion rotation);
        void Cleanup();
    }
}