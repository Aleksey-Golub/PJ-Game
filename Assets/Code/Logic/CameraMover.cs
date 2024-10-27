using UnityEngine;

internal class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;

    internal void SetTarget(Transform target)
    {
        _player = target;
    }

    private void LateUpdate()
    {
        transform.position = _player.position + _offset;
    }
}
