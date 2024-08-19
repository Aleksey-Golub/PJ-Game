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
    [SerializeField] private CircleCollider2D _collider2D;
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
    private ConfigsService _configsService;
    private Inventory _inventory;
    private ToolType _lastAbsentTool;

    #region EDITOR_ONLY

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetOverlapCircleCenter(), _overlapSphereParams.Radius);
    }
#endif
    #endregion

    private void Start()
    {
        var input = InputServiceProvider.Instance.GetService();
        var inventoryView = UIService.Instance.GetPlayerInventoryView();
        var configsService = ConfigsService.Instance;
        var popupFactory = PopupFactory.Instance;

        Construct(input, inventoryView, configsService, popupFactory);
    }

    private void Construct(IPlayerInput input, IInventoryView inventoryView, ConfigsService configsService, PopupFactory popupFactory)
    {
        _buffer = new Collider2D[10];
        _inventory = new();

        _input = input;
        _inventoryView = inventoryView;
        _inventoryView.Init(_inventory.Storage);
        _configsService = configsService;

        _view.Construct(popupFactory);

        _inventory.ResourceCountChanged += _inventoryView.UpdateFor;
        _view.AttackDone += OnHitDone;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        _view.AttackDone -= OnHitDone;
        _inventory.ResourceCountChanged -= _inventoryView.UpdateFor;
    }

    private void FixedUpdate()
    {
        //Logger.Log($"Player FixedUpdate at {Time.frameCount} frame");

        // movement
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

        // attack resource source
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

                    if (
                        collider.TryGetComponent(out ResourceSource s)
                        && !s.IsDied
                        )
                    {
                        if (CanGatherWith(s.NeedToolType))
                        {
                            sCount++;
                        }
                        else
                        {
                            _lastAbsentTool = s.NeedToolType;
                        }
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

        // trigger interaction
        Vector2 forTriggerCenter = (Vector2)transform.position + _collider2D.offset;
        if (Physics2D.OverlapCircleNonAlloc(forTriggerCenter, _collider2D.radius, _buffer) > 0)
        {
            foreach (Collider2D collider in _buffer)
            {
                if (collider == null)
                    continue;

                if (!collider.isTrigger)
                    continue;

                OnTrigger2D(collider);
            }

            _buffer.Refresh();
        }

        // handle last absent tool feature
        if (_lastAbsentTool != ToolType.None)
        {
            _view.ShowGatheringBlocked(_configsService.GetConfigFor(_lastAbsentTool).Sprite);
            _lastAbsentTool = ToolType.None;
        }
        else
        {
            _view.ShowGatheringUnblocked();
        }
    }

    private void OnTrigger2D(Collider2D other)
    {
        //Logger.Log($"OnTriggerStay2D with {other.gameObject.name} at {Time.frameCount} frame");

        if (other.TryGetComponent(out Resource resource))
        {
            _inventory.Add(resource.Type, resource.Count);
            _view.ShowCollect(resource.Type, resource.Count);
            resource.Collect();
        }

        if (other.TryGetComponent(out Tool tool))
        {
            _inventory.Add(tool.Type);
            tool.Collect();
        }

        if (other.TryGetComponent(out ResourceStorage resourceStorage))
        {
            if (CanGatherWith(resourceStorage.NeedToolType))
            {
                if (resourceStorage.CanInteract)
                    resourceStorage.Interact();
            }
            else
            {
                _lastAbsentTool = resourceStorage.NeedToolType;
            }
        }

        if (other.TryGetComponent(out IResourceConsumer resourceConsuner) && resourceConsuner.CanInteract)
        {
            InteractWith(resourceConsuner);
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

                if (
                    collider.TryGetComponent(out ResourceSource s)
                    && CanGatherWith(s.NeedToolType)
                    && !s.IsDied
                    )
                {
                    s.Interact();
                }
            }

            _buffer.Refresh();
        }
    }

    private bool CanGatherWith(ToolType needToolType)
    {
        if (needToolType is ToolType.None)
            return true;

        return _inventory.Has(needToolType);
    }

    private void InteractWith(IResourceConsumer resourceConsumer)
    {
        //Logger.Log($"Interact with {(resourceConsumer as MonoBehaviour).gameObject.name} {Time.frameCount}");

        // first visit

        // dummy implementation
        var needs = resourceConsumer.GetNeeds();
        if (_inventory.Has(needs.ResourceType, needs.CurrentNeedResourceCount))
        {
            _inventory.Remove(needs.ResourceType, needs.CurrentNeedResourceCount);

            resourceConsumer.Consume(needs.CurrentNeedResourceCount);
        }

        // revisitation
    }
}
