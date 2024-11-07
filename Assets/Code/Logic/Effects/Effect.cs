using UnityEngine;
using Code.Services;

public abstract class Effect : MonoBehaviour, IPoolable
{
    [SerializeField] protected Animator _hitEffectAnimator;
    [field: SerializeField] public EffectId EffectId { get; private set; }

    protected static readonly int _hitEffectHash = Animator.StringToHash("Hit Effect");
    protected static readonly int _explosionEffectHash = Animator.StringToHash("Explosion");

    private IRecyclableFactory _factory;

    public bool IsConstructed { get; private set; }

    #region EDITOR_ONLY
#if UNITY_EDITOR
    [Header("EDITOR_ONLY")]
    [SerializeField] private Sprite _toShowSprite;

    [ContextMenu(nameof(ShowEffect))]
    private void ShowEffect()
    {
        GetComponent<SpriteRenderer>().sprite = _toShowSprite;
    }

    [ContextMenu(nameof(HideEffect))]
    private void HideEffect()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }
#endif
    #endregion

    public void Construct(IRecyclableFactory factory)
    {
        _factory = factory;

        IsConstructed = true;
    }

    internal abstract void Play();
}

public enum EffectId
{
    None = 0,
    Explosion = 1,
    Hit = 2,
    HitPine = 3,
}