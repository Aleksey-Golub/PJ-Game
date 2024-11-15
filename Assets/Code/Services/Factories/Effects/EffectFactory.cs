using UnityEngine;
using System.Collections.Generic;

namespace Code.Services
{
    public class EffectFactory : IEffectFactory
    {
        private const string CONTAINER_NAME = "Effect Factory Container";

        private readonly Dictionary<EffectId, Pool<Effect>> _pools = new();
        private Transform _container;
        private readonly List<Effect> _inUseItems;
        private readonly IConfigsService _configs;

        public EffectFactory(IConfigsService configs)
        {
            _configs = configs;

            _inUseItems = new List<Effect>();
        }

        public void Load()
        {
            _container = FactoryHelper.CreateDontDestroyOnLoadGameObject(CONTAINER_NAME).transform;

            int poolSize = 10;
            foreach (EffectConfig c in _configs.EffectsConfigs.Values)
            {
                _pools[c.Template.EffectId] = new Pool<Effect>(c.Template, _container, poolSize);
            }
        }

        public void Cleanup()
        {
            _inUseItems.ForEach(i => _pools[i.EffectId].Recycle(i));
            _inUseItems.Clear();
            Object.Destroy(_container.gameObject);

            _pools.Clear();
        }

        public Effect Get(EffectId effectId, Transform template)
        {
            Pool<Effect> pool = _pools[effectId];

            Effect effect = pool.Get(template.position, template.rotation);

            if (!effect.IsConstructed)
                effect.Construct(this);

            effect.transform.localScale = template.localScale;

            _inUseItems.Add(effect);
            return effect;
        }

        public void Recycle(IPoolable poolable)
        {
            Effect effect = poolable as Effect;
            _inUseItems.Remove(effect);

            _pools[effect.EffectId].Recycle(effect);
        }
    }
}