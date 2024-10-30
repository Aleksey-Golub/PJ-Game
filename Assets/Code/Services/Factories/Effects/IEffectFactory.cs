using UnityEngine;

namespace Code.Services
{
    public interface IEffectFactory : IRecyclableFactory
    {
        /// <summary>
        /// Return effect. Use 'transformTemplate' as dto to pass position, rotation and scale.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="transformTemplate">Used as dto for position, rotation and scale</param>
        /// <returns></returns>
        Effect Get(EffectId effectId, Transform transformTemplate);
    }
}