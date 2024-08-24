using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private ConverterView _view;

    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _singleUpload = 5;
    [SerializeField] private int _maxUpload = 25;
    [SerializeField] private float _converTime = 10f;
    [SerializeField] private int _preferedConsumedValue = -1;

    [SerializeField] private ResourceConfig _dropResourceConfig;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;
    [SerializeField] private int _dropCount = 1;

    private float _timer;
    private int _currentUpload;
    private ResourceFactory _resourceFactory;

    public bool CanInteract => _currentUpload < _maxUpload;
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => _maxUpload - _currentUpload;

    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        Construct(resourceFactory);
        Init();
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    private void Construct(ResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;
    }

    internal void Init()
    {
        _currentUpload = 0;
        _timer = 0;

        _view.Init(_needResourceConfig.Sprite, _currentUpload, _dropResourceConfig.Sprite);
        _view.ShowNeeds(_singleUpload);
        _view.ShowUpload(_currentUpload, _maxUpload);
        _view.ShowProgress(_timer, _converTime);
    }

    private void OnUpdate(float deltaTime)
    {
        if (_currentUpload < _singleUpload)
            return;

        _timer += deltaTime;

        if (_timer >= _converTime)
        {
            _timer = 0;
            
            _currentUpload -= _singleUpload;
            _view.ShowUpload(_currentUpload, _maxUpload);
            DropResource();
        }

        _view.ShowProgress(_timer, _converTime);
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = _maxUpload - _currentUpload
        };
    }

    public void Consume(int value)
    {
        _currentUpload += value;
        _view.ShowUpload(_currentUpload, _maxUpload);
    }

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, _dropSettings, _dropCount, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource dropObject = _resourceFactory.Get(transform.position, Quaternion.identity);
            dropObject.Init(_dropResourceConfig, dropData[i].ResourceInPackCount);

            dropObject.MoveAfterDrop(dropData[i]);
        }
    }
}
