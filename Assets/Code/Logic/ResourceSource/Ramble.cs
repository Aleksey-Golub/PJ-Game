using UnityEngine;

internal class Ramble : MonoBehaviour
{
    [SerializeField] private RambleMoverBase _mover;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _rambleBoolName = "Jump";

    [Header("Settings")]
    [SerializeField] private Vector2 _stayTime = new Vector2(2, 5);
    [SerializeField] private Vector2 _radiusToMove = new Vector2(1, 2);

    private Vector3 _targetPoint;
    private bool _isMoving;
    private float _stayTimer;
    private float _stayDelay;
    private int _rambleHash;

    private void Awake()
    {
        _rambleHash = Animator.StringToHash(_rambleBoolName);
        _mover.Reached += OnMoverReached;

        SetNewStayDelay();
    }

    private void OnDestroy()
    {
        _mover.Reached -= OnMoverReached;
    }

    private void Update()
    {
        _stayTimer += Time.deltaTime;

        if (!_isMoving && _stayTimer >= _stayDelay)
        {
            _targetPoint = GetTargetPoint();

            _isMoving = true;
            _animator.SetBool(_rambleHash, true);
        }

        if (_isMoving)
        {
            _mover.MoveTo(_targetPoint);
        }
    }

    private void OnMoverReached()
    {
        _isMoving = false;
        _animator.SetBool(_rambleHash, false);
        _stayTimer = 0;
    }

    private Vector3 GetTargetPoint()
    {
        Vector3 result;
        int k = 0;

        do
        {
            var radius = UnityEngine.Random.Range(_radiusToMove.x, _radiusToMove.y);
            result = transform.position + UnityEngine.Random.insideUnitSphere * radius;
            result.z = transform.position.z;

            k++;

            if (IsBreaked())
                break;
        }
        while (!_mover.IsValid(targetPoint: result));

        return IsBreaked() ? transform.position : result;

        bool IsBreaked() => k > 10;
    }

    private void SetNewStayDelay()
    {
        _stayDelay = UnityEngine.Random.Range(_stayTime.x, _stayTime.y);
    }
}
