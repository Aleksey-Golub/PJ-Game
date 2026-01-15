using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

internal class Ramble : MonoBehaviour
{
    [SerializeField] private RambleMoverBase _mover;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _rambleBoolName = "Jump";

    [Header("Settings")]
    [Tooltip("Tries count to find target point before use self position instead")]
    [SerializeField] private int _triesToFindTargetPoint = 10;
    [Tooltip("Used to stop endless duration. Usefull set as Max move radius devided by Speed")]
    [SerializeField] private float _moveMaxDuration = 10;
    [SerializeField] private Vector2 _stayTime = new Vector2(2, 5);
    [SerializeField] private Vector2 _radiusToMove = new Vector2(1, 2);

    private Vector3 _targetPoint;
    private bool _isMoving;
    private float _moveTimer;
    private float _stayTimer;
    private float _stayDelay;
    private int _rambleHash;

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 selfPosition = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_targetPoint, 0.15f);
        Gizmos.DrawLine(selfPosition, _targetPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(selfPosition, _radiusToMove.x);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(selfPosition, _radiusToMove.y);
    }
#endif
    #endregion

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
        float deltaTime = Time.deltaTime;

        _stayTimer += deltaTime;
        if (!_isMoving && _stayTimer >= _stayDelay)
        {
            _targetPoint = GetTargetPoint();

            _isMoving = true;
            _animator.SetBool(_rambleHash, true);
        }

        if (_isMoving)
        {
            _moveTimer += deltaTime;
            _mover.MoveTo(_targetPoint);

            if (_moveTimer >= _moveMaxDuration)
                OnMoverReached();
        }
    }

    private void OnMoverReached()
    {
        _isMoving = false;
        _animator.SetBool(_rambleHash, false);
        _stayTimer = 0;
        _moveTimer = 0;
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

        bool IsBreaked() => k > _triesToFindTargetPoint;
    }

    private void SetNewStayDelay()
    {
        _stayDelay = UnityEngine.Random.Range(_stayTime.x, _stayTime.y);
    }
}
