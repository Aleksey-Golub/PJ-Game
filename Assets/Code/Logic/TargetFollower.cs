using UnityEngine;

internal class TargetFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    internal void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        transform.position = _target.position + _offset;
    }
}
