using System.Collections.Generic;
using UnityEngine;

internal class ResourceMergeService : MonoSingleton<ResourceMergeService>
{
    [SerializeField] private float _distanceToMerge = 1.5f;
    [SerializeField] private float _timeToMerge = 1f;

    private ResourceFactory _resourceFactory;
    
    private float DistanceToMergeSquare => _distanceToMerge * _distanceToMerge;

    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        Construct(resourceFactory);
    }

    private void Construct(ResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;
    }

    private void Update()
    {
        MergeDroppedResources();
    }

    private void MergeDroppedResources()
    {
        IReadOnlyList<IMergingResource> droppedResources = _resourceFactory.DroppedResources;

        foreach (var res in droppedResources)
        {
            res.UpdateDroppedTime(Time.deltaTime);
        }

        for (int i = droppedResources.Count - 1; i >= 0; i--)
        {
            IMergingResource current = droppedResources[i];
            if (!current.IsReadyToMerge(_timeToMerge))
                continue;

            IMergingResource candidateToMergeTo = null;
            float minFondSqrDistanse = DistanceToMergeSquare;
            for (int n = i - 1; n >= 0; n--)
            {
                IMergingResource next = droppedResources[n];
                if (!next.IsReadyToMerge(_timeToMerge))
                    continue;

                if (next.Type != current.Type)
                    continue;

                Vector3 fromTo = current.Position - next.Position;
                float fromToSqrMagnitude = fromTo.sqrMagnitude;
                if (fromToSqrMagnitude <= minFondSqrDistanse)
                {
                    minFondSqrDistanse = fromToSqrMagnitude;
                    candidateToMergeTo = next;
                }
            }

            if (candidateToMergeTo != null)
                DoMerge(current, candidateToMergeTo);
        }
    }

    private void DoMerge(IMergingResource from, IMergingResource to)
    {
        to.SetCount(to.Count + from.Count);
        from.SetCount(0);
        from.MoveAfterMerge(to.Position);
    }
}
