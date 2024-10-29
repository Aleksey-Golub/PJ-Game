using System;
using UnityEngine;

internal abstract class RambleMoverBase : MonoBehaviour
{
    [SerializeField] protected float Speed = 1f;

    internal event Action Reached;

    internal abstract void MoveTo(Vector3 targetPosition);
    internal abstract bool IsValid(Vector3 targetPoint);

    protected void InvokeReached() => Reached?.Invoke();
}
