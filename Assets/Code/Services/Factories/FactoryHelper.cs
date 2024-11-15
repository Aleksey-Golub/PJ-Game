using UnityEngine;

namespace Code.Services
{
    public static class FactoryHelper
    {
        public static GameObject CreateDontDestroyOnLoadGameObject(string name)
        {
            var go = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go;
        }
    }
}