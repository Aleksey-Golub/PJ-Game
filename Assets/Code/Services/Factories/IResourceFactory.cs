using System.Collections.Generic;
using UnityEngine;

namespace Code.Services
{
    interface IResourceFactory : IRecyclableFactory
    {
        IReadOnlyList<Resource> DroppedResources { get; }

        Resource Get(Vector3 position, Quaternion rotation);
    }
}