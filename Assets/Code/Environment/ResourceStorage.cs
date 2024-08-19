﻿using System;
using UnityEngine;

internal class ResourceStorage : MonoBehaviour
{
    [SerializeField] private ResourceStorageView _view;

    [Header("Settings")]
    [SerializeField] private ToolType _needToolType = ToolType.None;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [SerializeField] private int _startResourceCount = 1;
    [SerializeField] private float _restoreTime = 10;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;

    private ResourceFactory _resourceFactory;

    private float _restorationTimer = 0;
    private int _currentResourceCount;

    private bool IsFull => _currentResourceCount >= _dropResourceCount;
    private bool IsSingleUse => _restoreTime < 0;
    internal bool CanInteract => _currentResourceCount > 0;
    internal ToolType NeedToolType => _needToolType;

    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        Construct(resourceFactory);
    }

    private void Construct(ResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;

        _currentResourceCount = _startResourceCount;
        _view.ShowResourceCount(_currentResourceCount, _dropResourceCount);
        _view.ShowWhole();
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        if (IsSingleUse)
        {
            enabled = (false);
            return;
        }

        if (IsFull)
            return;

        _restorationTimer += Time.deltaTime;

        if (_restorationTimer >= _restoreTime && _currentResourceCount < _dropResourceCount)
        {
            _restorationTimer = 0;
            Restore(1);
        }
    }

    internal virtual void Interact()
    {
        //Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _view.ShowHitEffect();
        _view.PlayHitSound();
        _view.ShowHitAnimation();

        DropResource();
        Exhaust();
    }

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, _dropSettings, _currentResourceCount, out int notFittedInPacksCount);
        Restore(notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }
    }

    private void Exhaust()
    {
        _currentResourceCount = 0;
        _view.ShowResourceCount(_currentResourceCount, _dropResourceCount);
        _view.ShowExhaust();

        if (IsSingleUse)
            Invoke(nameof(DisableCollider), 1f);
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }

    private void Restore(int value)
    {
        if (value == 0)
            return;

        _currentResourceCount += value;
        _view.ShowResourceCount(_currentResourceCount, _dropResourceCount);
        _view.ShowWhole();
    }
}
