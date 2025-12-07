internal class ExplosionEffect : Effect
{
    internal override void Play()
    {
        base.Play();

        _hitEffectAnimator.Play(_explosionEffectHash);
    }
}
