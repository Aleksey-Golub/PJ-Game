using System;
using UnityEngine;

internal class Slime : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Settings")]
    [SerializeField] private Vector2 _stayTime = new Vector2(2, 5);
    [SerializeField] private Vector2 _radiusToMove = new Vector2(1, 2);
    [SerializeField] private float _speed = 1f;
    private Vector3 _targetPoint;
    private bool _isMoving;
    private float _stayTimer;
    private float _stayDelay;
    private int _jumpHash;

    private void Awake()
    {
        _jumpHash = Animator.StringToHash("Jump");
        SetNewStayDelay();
    }

    private void Update()
    {
        _stayTimer += Time.deltaTime;

        if (!_isMoving && _stayTimer >= _stayDelay)
        {
            _targetPoint = GetTargetPoint();

            _isMoving = true;
            _animator.SetBool(_jumpHash, true);
        }
        
        if (_isMoving)
            MoveTo(_targetPoint);
    }

    private void MoveTo(Vector3 targetPoint)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, _speed * Time.deltaTime);

        if (transform.position == targetPoint)
        {
            _isMoving = false;
            _animator.SetBool(_jumpHash, false);
            _stayTimer = 0;
        }
    }

    private Vector3 GetTargetPoint()
    {
        Vector3 result;

        var radius = UnityEngine.Random.Range(_radiusToMove.x, _radiusToMove.y);
        result = transform.position + UnityEngine.Random.insideUnitSphere * radius;
        result.z = transform.position.z;

        return IsValid(result) ? result : GetTargetPoint();

        bool IsValid(Vector3 result)
        {
            return true;
        }
    }

    private void SetNewStayDelay()
    {
        _stayDelay = UnityEngine.Random.Range(_stayTime.x, _stayTime.y);
    }
}
