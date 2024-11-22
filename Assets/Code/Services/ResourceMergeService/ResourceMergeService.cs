using System.Collections.Generic;
using UnityEngine;

namespace Code.Services
{
    internal class ResourceMergeService : IResourceMergeService
    {
        private const string CONFIG_PATH = "Infrastructure/Configs/ResourceMergeServiceConfig";
        
        private readonly IResourceFactory _resourceFactory;
        private ResourceMergeServiceConfig _config;

        private float DistanceToMergeSquare => _config.DistanceToMerge * _config.DistanceToMerge;

        internal ResourceMergeService(IResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public void Load()
        {
            _config = Resources.Load<ResourceMergeServiceConfig>(CONFIG_PATH);
        }

        public void OnUpdate(float deltaTime)
        {
            MergeDroppedResources(deltaTime);
        }

        private void MergeDroppedResources(float deltaTime)
        {
            IReadOnlyList<IMergingResource> droppedResources = _resourceFactory.DroppedResources;

            foreach (var res in droppedResources)
            {
                res.UpdateDroppedTime(deltaTime);
            }

            for (int i = droppedResources.Count - 1; i >= 0; i--)
            {
                IMergingResource current = droppedResources[i];
                if (!current.IsReadyToMerge(_config.TimeToMerge))
                    continue;

                IMergingResource candidateToMergeTo = null;
                float minFondSqrDistanse = DistanceToMergeSquare;
                for (int n = i - 1; n >= 0; n--)
                {
                    IMergingResource next = droppedResources[n];
                    if (!next.IsReadyToMerge(_config.TimeToMerge))
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

    internal interface IResourceMergeService : IService, IUpdatable
    {
        void Load();
    }
}