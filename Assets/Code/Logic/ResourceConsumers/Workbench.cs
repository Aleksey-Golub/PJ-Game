using Code.Services;
using UnityEngine;

[SelectionBase]
public class Workbench : SingleUseConsumerBase<ResourceConsumerView>
{
    [SerializeField] private ScriptableObject _dropConfigMono;
    [field: SerializeField] public DropSettings DropSettings { get; private set; } = DropSettings.Default;
    [SerializeField] private int _dropCount = 1;

    private IDropObjectConfig _dropConfig;
    private IResourceFactory _resourceFactory;
    private IToolFactory _toolFactory;

    private void OnValidate()
    {
        if (_dropConfigMono is not null && _dropConfigMono is not IDropObjectConfig config)
        {
            _dropConfigMono = null;
            Debug.LogError($"Wrong config used. Set IDropObjectConfig");
        }
    }

    private void Start()
    {
        var resourceFactory = AllServices.Container.Single<IResourceFactory>();
        var toolFactory = AllServices.Container.Single<IToolFactory>();
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(resourceFactory, toolFactory, audio, effectFactory);
        Init();
    }

    private void Construct(IResourceFactory resourceFactory, IToolFactory toolFactory, IAudioService audio, IEffectFactory effectFactory)
    {
        Construct();

        _resourceFactory = resourceFactory;
        _toolFactory = toolFactory;

        _dropConfig = _dropConfigMono as IDropObjectConfig;
        View.Construct(audio, effectFactory);
    }

    protected override Sprite GetGenerateObjSprite() => _dropConfig.Sprite;

    protected override void DropObject()
    {
        View.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, DropSettings, _dropCount, out int notFittedInPacksCount);

        if (_dropConfig is ResourceConfig resourceConfig)
        {
            for (int i = 0; i < dropData.Count; i++)
            {
                Resource dropObject = _resourceFactory.Get(transform.position, Quaternion.identity);
                dropObject.Init(resourceConfig, dropData[i].ResourceInPackCount);

                dropObject.MoveAfterDrop(dropData[i]);
            }
        }
        else if (_dropConfig is ToolConfig toolConfig)
        {
            for (int i = 0; i < dropData.Count; i++)
            {
                Tool dropObject = _toolFactory.Get(transform.position, Quaternion.identity);
                dropObject.Init(toolConfig);

                dropObject.MoveAfterDrop(dropData[i]);
            }
        }
        else
        {
            Logger.LogError($"[Workbench] DropObject() error : 'Not implemented for {_dropConfig.GetType()}'");
        }
    }
}

