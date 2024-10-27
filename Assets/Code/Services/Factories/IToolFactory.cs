using UnityEngine;

namespace Code.Services
{
    internal interface IToolFactory : IRecyclableFactory
    {
        Tool Get(Vector3 position, Quaternion rotation);
    }
}