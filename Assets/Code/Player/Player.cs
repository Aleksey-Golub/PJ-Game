using Code.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.UI;
using Code.Data;
using Code.Infrastructure;

public class Player : MonoBehaviour, IDisposable, ISavedProgressReader, ISavedProgressWriter
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
    [SerializeField] private float _consumeDelay = 0.2f;

    private float _consumeTimer = 0;
    private float _attackTimer = 0;
    private Vector2 _direction = new Vector2(0, -1);
    private Collider2D[] _buffer;
    private IInputService _input;
    private IInventoryView _inventoryView;
    private IConfigsService _configsService;
    private ITransitionalResourceFactory _transitionalResourceFactory;
    private IPersistentProgressService _progressService;
    private Inventory _inventory;
    private ToolType _lastAbsentTool;
    private Dictionary<IResourceConsumer, ResourceConsumerNeeds> _lastConsumersData;
    private HashSet<IResourceConsumer> _currentConsumers = new();

    private SellBoard _sellBoard;
    private bool _inSellBoard;
    private UpgradeBoard _upgradeBoard;
    private bool _inUpgradeBoard;

    private bool _isSpeedUp;
    private float _speedUpSpeed = 1f;
    private Timer _speedUpTimer;

    private float CurrentSpeed => _isSpeedUp ? _speedUpSpeed : _speed;

    public Inventory Inventory => _inventory;

    #region EDITOR_ONLY Draw interact sphere

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetOverlapCircleCenter(), _overlapSphereParams.Radius);
    }
#endif
    #endregion

    internal void Construct(
        IInputService input,
        IInventoryView inventoryView,
        IConfigsService configsService,
        IPopupFactory popupFactory,
        ITransitionalResourceFactory transitionalResourceFactory,
        IPersistentProgressService progressService)
    {
        _buffer = new Collider2D[10];
        _inventory = new();
        _lastConsumersData = new();
        _currentConsumers = new();

        _input = input;
        _inventoryView = inventoryView;
        _inventoryView.Init(_inventory.Storage);
        _configsService = configsService;
        _transitionalResourceFactory = transitionalResourceFactory;

        _progressService = progressService;

        _view.Construct(popupFactory);

        _inventory.ResourceCountChanged += _inventoryView.UpdateFor;
        _view.AttackDone += OnHitDone;

        _speedUpTimer = new Timer();
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

    #region Progress Read - Write
    void ISavedProgressWriter.WriteToProgress(GameProgress progress)
    {
        WriteToPositionOnLevel(progress);
        WriteInventoryData(progress);
        WriteSpeedUpData(progress);
    }

    void ISavedProgressReader.ReadProgress(GameProgress progress)
    {
        ReadPositionOnLevel(progress);
        ReadInventoryData(progress);
        ReadSpeedUpData(progress);
    }

    private void WriteInventoryData(GameProgress progress)
    {
        _inventory.WriteToProgress(progress);
    }

    private void ReadInventoryData(GameProgress progress)
    {
        _inventory.ReadProgress(progress);
        _inventoryView.Init(_inventory.Storage);
    }

    private void WriteToPositionOnLevel(GameProgress progress)
    {
        progress.PlayerProgress.PositionOnLevel =
            new PositionOnLevel(
                CurrentLevel(),
                transform.position.AsVectorData(),
                _direction.AsVectorData()
                );
    }

    private void ReadPositionOnLevel(GameProgress progress)
    {
        if (CurrentLevel() != progress.PlayerProgress.PositionOnLevel.Level)
            return;

        PositionOnLevel positionOnLevel = progress.PlayerProgress.PositionOnLevel;
        Vector3Data savedPosition = positionOnLevel.Position;
        if (savedPosition != null)
            transform.position = savedPosition.AsUnityVector();

        Vector2Data savedDirection = positionOnLevel.Direction;
        if (savedDirection != null)
        {
            _direction = savedDirection.AsUnityVector();
            _view.PlayMove(_direction, 0);
        }
    }

    private void WriteSpeedUpData(GameProgress progress)
    {
        progress.PlayerProgress.SpeedUpTimerData = new TimerData(_speedUpTimer.IsStarted, _speedUpTimer.Passed, _speedUpTimer.Duration);
        progress.PlayerProgress.SpeedUpSpeed = _speedUpSpeed;
    }

    private void ReadSpeedUpData(GameProgress progress)
    {
        var timerData = progress.PlayerProgress.SpeedUpTimerData;
        float speed = progress.PlayerProgress.SpeedUpSpeed;

        if (timerData.IsStarted)
        {
            SpeedUp(speed, timerData.Duration, timerData.Passed);
        }
    }
    #endregion

    internal void Teleport(Vector3 to)
    {
        _view.BeforeTeleport();
        //_rb.MovePosition(to);
        transform.position = to;
        _view.AfterTeleport();
    }

    internal void SpeedUp(float newSpeed, float time, float passed = 0)
    {
        if (!_isSpeedUp)
        {
            _isSpeedUp = true;
            _speedUpTimer.Elapsed += OnSpeedUpTimerElapsed;
            _view.ShowSpeedUp();
        }

        _speedUpSpeed = newSpeed;
        _speedUpTimer.StartAsPartialPassed(time, passed);
    }

    private void OnSpeedUpTimerElapsed(Timer speedUpTimer)
    {
        speedUpTimer.Elapsed -= OnSpeedUpTimerElapsed;

        _isSpeedUp = false;
        _view.HideSpeedUp();
    }

    internal void SetLastAbsentTool(ToolType type)
    {
        _lastAbsentTool = type;
    }

    private void FixedUpdate()
    {
        //Logger.Log($"Player FixedUpdate at {Time.frameCount} frame");

        // movement
        _speedUpTimer.OnUpdate(Time.fixedDeltaTime);

        if (_input.HasMoveInput())
        {
            float xMovement = _input.GetHorizontalAxisRaw();
            float yMovement = _input.GetVerticalAxisRaw();

            Vector2 direction = new Vector2(xMovement, yMovement).normalized;
            Vector2 startPos = _rb.position;
            Vector2 offset = direction * CurrentSpeed * Time.fixedDeltaTime;

            _rb.MovePosition(startPos + offset);
            _view.PlayMove(direction.x, direction.y, direction.magnitude);

            _direction = direction;
        }
        else
        {
            _view.PlayMove(_direction, 0);
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
                        if (Has(s.NeedToolType))
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
            _currentConsumers.Clear();
            _inSellBoard = false;
            _inUpgradeBoard = false;
            foreach (Collider2D collider in _buffer)
            {
                if (collider == null)
                    continue;

                if (!collider.isTrigger)
                    continue;

                OnTrigger2D(collider);
            }
            HandleConsumers();
            HandleSellBoard();
            HandleUpgradeBoard();

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
            if (!_inventory.Has(tool.Type))
                _inventory.Add(tool.Type);

            tool.Collect();
            string toolID = _configsService.GetConfigFor(tool.Type).ID;

            UpgradeItemsProgress upgradeItemsProgress = _progressService.Progress.PlayerProgress.UpgradeItemsProgress;
            upgradeItemsProgress.TryGet(toolID, out int value);
            if (value == 0)
                upgradeItemsProgress.Upgrade(toolID);
        }

        if (other.TryGetComponent(out SellBoard sellBoard))
        {
            _sellBoard = sellBoard;
            _inSellBoard = true;
        }

        if (other.TryGetComponent(out UpgradeBoard upgradeBoard))
        {
            _upgradeBoard = upgradeBoard;
            _inUpgradeBoard = true;
        }

        if (other.TryGetComponent(out DungeonEntrance entrance))
        {
            if (entrance.CanInteract(this))
                entrance.Interact(this);
        }

        if (other.TryGetComponent(out ResourceStorage resourceStorage))
        {
            if (Has(resourceStorage.NeedToolType))
            {
                if (resourceStorage.CanInteract)
                    resourceStorage.Interact();
            }
            else
            {
                _lastAbsentTool = resourceStorage.NeedToolType;
            }
        }

        if (other.TryGetComponent(out IResourceConsumer resourceConsumer) && resourceConsumer.CanInteract)
        {
            //Logger.Log($"Interact with {(resourceConsumer as MonoBehaviour).gameObject.name} {Time.frameCount}");
            _currentConsumers.Add(resourceConsumer);
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
                    && Has(s.NeedToolType)
                    && !s.IsDied
                    )
                {
                    s.Interact();
                }
            }

            _buffer.Refresh();
        }
    }

    public bool Has(ToolType needToolType)
    {
        if (needToolType is ToolType.None)
            return true;

        return _inventory.Has(needToolType);
    }

    private void HandleConsumers()
    {
        _consumeTimer += Time.fixedDeltaTime;
        if (_consumeTimer < _consumeDelay)
        {
            return;
        }

        var lastConsumers = _lastConsumersData.Keys.ToHashSet();

        // nothing to handle
        if (lastConsumers.Count == 0 && _currentConsumers.Count == 0)
            return;

        // remove not used consumers
        foreach (var last in lastConsumers)
        {
            if (!_currentConsumers.Contains(last))
                _lastConsumersData.Remove(last);
        }

        _currentConsumers.ExceptWith(lastConsumers);

        // add new consumers
        foreach (var @new in _currentConsumers)
        {
            _lastConsumersData.Add(@new, @new.GetNeeds());
        }
        _currentConsumers.Clear();

        // do consume
        int consumersHandledCount = 0;
        foreach (var cPair in _lastConsumersData)
        {
            var needs = cPair.Value;
            IResourceConsumer consumer = cPair.Key;
            var resourceType = needs.ResourceType;
            _inventory.GetCount(resourceType, out int inInventoryCount);
            int consumedValue = GetConsumesValue(inInventoryCount, needs.CurrentNeedResourceCount, consumer.PreferedConsumedValue);
            consumedValue = Mathf.Min(consumedValue, consumer.FreeSpace);

            if (consumedValue == 0)
                continue;

            if (_inventory.Has(resourceType, consumedValue))
            {
                _inventory.Reserve(resourceType, consumedValue);

                var transitionalResource = _transitionalResourceFactory.Get(transform.position, Quaternion.identity);
                transitionalResource.Init(_inventory, resourceType, consumer, consumedValue, _configsService.GetConfigFor(resourceType).Sprite);
                transitionalResource.MoveTo(consumer.TransitionalResourceFinalPosition);

                consumer.ApplyPreUpload(consumedValue);
                consumersHandledCount++;
            }
        }

        if (consumersHandledCount > 0)
            _consumeTimer = 0;

        // locals
        static int GetConsumesValue(int inInventoryCount, int currentNeedResourceCount, int preferedConsumedValue)
        {
            int packSize = 0;
            if (preferedConsumedValue < 1)
                packSize = currentNeedResourceCount / 8 + 1;
            else
                packSize = preferedConsumedValue;

            return inInventoryCount < packSize ? inInventoryCount : packSize;
        }
    }

    private void HandleSellBoard()
    {
        if (_sellBoard == null)
            return;

        if (_inSellBoard)
        {
            if (!_sellBoard.IsVisited)
                _sellBoard.Open(_inventory);
        }
        else
        {
            _sellBoard.Close();
            _sellBoard = null;
        }
    }

    private void HandleUpgradeBoard()
    {
        if (_upgradeBoard == null)
            return;

        if (_inUpgradeBoard)
        {
            if (!_upgradeBoard.IsVisited)
                _upgradeBoard.Open(_inventory);
        }
        else
        {
            _upgradeBoard.Close();
            _upgradeBoard = null;
        }
    }

    private string CurrentLevel() => SceneLoader.CurrentLevel();
}

