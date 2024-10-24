internal class ExplosionEffect : Effect
{
    internal override void Play()
    {
        _hitEffectAnimator.Play(_explosionEffectHash);
    }
}
