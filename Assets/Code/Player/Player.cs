using System;
using UnityEngine;

internal class Player : MonoBehaviour, IDisposable
{
    [Serializable]
    internal struct OverlapSphereParams
    {
        public float Radius;
        public float Offset;
    }

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private PlayerView _view;

    [Header("Settings")]
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _attackDelay = 0.7f;
    [SerializeField] private OverlapSphereParams _overlapSphereParams = new OverlapSphereParams() { Radius = 0.4f, Offset = 0.4f };

    private float _attackTimer = 0;
    private Vector2 _direction = new Vector2(0, -1);
    private Collider2D[] _buffer;
    private IPlayerInput _input;
    private IInventoryView _inventoryView;
    private Inventory _inventory;

    #region EDITOR_ONLY

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetOverlapCircleCenter(), _overlapSphereParams.Radius);
    }
#endif
    #endregion

    private void Construct(IPlayerInput input, IInventoryView inventoryView)
    {
        _buffer = new Collider2D[10];
        _inventory = new();

        _input = input;
        _inventoryView = inventoryView;
        _inventoryView.Init(_inventory.Storage);

        _view.Construct();

        _inventory.ResourceCountChanged += _inventoryView.UpdateFor;
        _view.AttackDone += OnHitDone;
    }

    private void Start()
    {
        var input = InputServiceProvider.Instance.GetService();
        var inventoryView = UIService.Instance.GetPlayerInventoryView();
        Construct(input, inventoryView);
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Resource resource))
        {
            _inventory.Add(resource.Type, resource.Count);
            resource.Collect();
        }
    }

    public void Dispose()
    {
        _view.AttackDone -= OnHitDone;
        _inventory.ResourceCountChanged -= _inventoryView.UpdateFor;
    }

    private void FixedUpdate()
    {
        if (_input.HasMoveInput())
        {
            float xMovement = _input.GetHorizontalAxisRaw();
            float yMovement = _input.GetVerticalAxisRaw();

            Vector2 direction = new Vector2(xMovement, yMovement).normalized;
            Vector2 startPos = _rb.position;
            Vector2 offset = direction * _speed * Time.fixedDeltaTime;

            _rb.MovePosition(startPos + offset);
            _view.PlayMove(direction.x, direction.y, direction.magnitude);

            _direction = direction;
        }
        else
        {
            _view.PlayMove(_direction.x, _direction.y, 0);
        }

        _attackTimer += Time.fixedDeltaTime;
        if (_attackTimer >= _attackDelay)
        {
            Vector2 center = GetOverlapCircleCenter();

            if (Physics2D.OverlapCircleNonAlloc(center, _overlapSphereParams.Radius, _buffer) > 0)
            {
                int sCount = 0;
                foreach (Collider2D collider in _buffer)
                {
                    if (collider == null)
                        continue;

                    if (collider.TryGetComponent<Player>(out _))
                        continue;

                    if (collider.TryGetComponent(out ResourceSource s))
                    {
                        sCount++;
                    }
                }

                if (sCount > 0)
                {
                    _attackTimer = 0;
                    _view.PlayAttack();
                }

                _buffer.Refresh();
            }
        }
    }

    private Vector2 GetOverlapCircleCenter()
    {
        Vector2 center = (Vector2)transform.position + _collider2D.offset;
        center += _direction * _overlapSphereParams.Offset;
        return center;
    }

    private void OnHitDone()
    {
        Vector2 center = GetOverlapCircleCenter();

        if (Physics2D.OverlapCircleNonAlloc(center, _overlapSphereParams.Radius, _buffer) > 0)
        {
            foreach (Collider2D collider in _buffer)
            {
                if (collider == null)
                    continue;

                if (collider.TryGetComponent<Player>(out _))
                    continue;

                if (collider.TryGetComponent(out ResourceSource s))
                {
                    s.Interact();
                }
            }

            _buffer.Refresh();
        }
    }
}
