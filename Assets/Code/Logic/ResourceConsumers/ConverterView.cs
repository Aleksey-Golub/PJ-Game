using Code.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

internal class ConverterView : ResourceConsumerView
{
    [Header("Progress")]
    [SerializeField] private GameObject _progress;
    [SerializeField] private GameObject _progressFg;
    
    [Header("Upload")]
    [SerializeField] private ResourceConsumerUploadView[] _uploadViews;

    private Dictionary<ResourceType, ResourceConsumerUploadView> _uploadViewsCached;

    internal override void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _uploadViewsCached = _uploadViews.ToDictionary(v => v.ResourceType, v => v);

        base.Construct(audio, effectFactory);
    }

    internal void Init(ResourceConsumerNeedSettings[] needSettings, Dictionary<ResourceType, ResourceConsumerNeedData> needData, DropResourceSettings[] dropResourceSettings)
    {
        foreach (var needSetting in needSettings)
        {
            ResourceType type = needSetting.NeedResourceConfig.Type;
            ResourceConsumerNeedView needView = _needViewsCached[type];

            needView.ResourceNeedImage.sprite = needSetting.NeedResourceConfig.Sprite;
            needView.NeedText.text = needData[type].CurrentUpload.ToString();
        }

        foreach (var dropSetting in dropResourceSettings)
        {
            ResourceType type = dropSetting.DropResourceConfig.Type;
            ResourceConsumerDropView dropView = _dropViewsCached[type];

            dropView.GenerateObjImage.sprite = dropSetting.DropResourceConfig.Sprite;
        }
    }

    internal void ShowUpload(ResourceType resourceType, int currentUpload, Func<ResourceType, int> getMaxUpload)
    {
        int maxUpload = getMaxUpload(resourceType);

        _uploadViewsCached[resourceType].UploadText.text = $"{currentUpload}/{maxUpload}";
    }

    internal void ShowProgress(float timer, float converTime)
    {
        float t = timer / converTime;

        Vector3 newScale = _progressFg.transform.localScale;
        newScale.x = t;
        _progressFg.transform.localScale = newScale;

        _progress.SetActive(timer > 0);
    }
}

[Serializable]
public class ResourceConsumerUploadView
{
    [field: SerializeField] public ResourceType ResourceType { get; private set; }
    [field: SerializeField] public TextMeshPro UploadText { get; private set; }
}
