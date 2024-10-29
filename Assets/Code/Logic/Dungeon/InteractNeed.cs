using UnityEngine;

public abstract class InteractNeed : ScriptableObject
{
    internal abstract bool CanInteract(Player player);
}
