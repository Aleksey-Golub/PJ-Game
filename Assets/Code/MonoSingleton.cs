using UnityEngine;

internal abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    internal static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
        }
        else
        {
            Destroy(this);
        }
    }
    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
