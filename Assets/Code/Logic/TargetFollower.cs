using UnityEngine;

internal class TargetFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    [Header("Smoothing")]
    [SerializeField] private bool _smoothing;
    [SerializeField] private float _smoothTime = 0.15f;
    
    private Vector3 _velocity;
    
    private void LateUpdate()
    {
        var desiredPos = _target.position + _offset;

        if (!_smoothing)
        {
            transform.position = desiredPos;
            _velocity = Vector3.zero;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _velocity, _smoothTime);
    }

    internal void SetTarget(Transform target)
    {
        _target = target;
    }
}
