using UnityEngine;

internal class HitEffect : Effect
{
    [SerializeField] private Transform _childEffect;
    [SerializeField] private Vector2 _offset = new Vector2(0, 0.1f);
    [SerializeField] private float _spreadRadius = 0.2f;
    
    internal override void Play()
    {
        _childEffect.localPosition = _offset + Random.insideUnitCircle * _spreadRadius;
        _hitEffectAnimator.Play(_hitEffectHash);
    }
}
