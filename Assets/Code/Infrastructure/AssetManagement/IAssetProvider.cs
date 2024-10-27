using Code.Services;
using UnityEngine;

namespace Code.Infrastructure
{
    public interface IAssetProvider : IService
    {
        GameObject Instantiate(string path, Transform parent);
        GameObject Instantiate(string path, Vector3 at);
        GameObject Instantiate(string path);
        T Load<T>(string path) where T : Object;
    }
}