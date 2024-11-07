using System.Collections.Generic;
using UnityEngine;

namespace Code.Services
{
    public interface IToolFactory : IRecyclableFactory
    {
        IReadOnlyList<Tool> DroppedResources { get; }
        void Load();
        Tool Get(Vector3 position, Quaternion rotation);
        void Cleanup();
    }
}