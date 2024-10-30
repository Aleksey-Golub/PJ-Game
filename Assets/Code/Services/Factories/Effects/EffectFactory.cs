using UnityEngine;
using System.Collections.Generic;

namespace Code.Services
{
    public class EffectFactory : IEffectFactory
    {
        private readonly Dictionary<EffectId, Pool<Effect>> _pools = new();

        public EffectFactory(IAudioService audio, IConfigsService configs)
        {
            Transform container = CreateContainer();

            int poolSize = 10;
            foreach (EffectConfig c in configs.EffectsConfigs.Values)
            {
                _pools[c.Template.EffectId] = new Pool<Effect>(c.Template, container, poolSize, this, audio);
            }
        }

        public Effect Get(EffectId effectId, Transform template)
        {
            Pool<Effect> pool = _pools[effectId];

            var effect = pool.Get(template.position, template.rotation);
            effect.transform.localScale = template.localScale;

            return effect;
        }

        public void Recycle(IPoolable poolable)
        {
            Effect effect = poolable as Effect;

            _pools[effect.EffectId].Recycle(effect);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Effect Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}