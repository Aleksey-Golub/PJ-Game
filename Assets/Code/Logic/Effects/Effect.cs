using UnityEngine;

internal abstract class Effect : MonoBehaviour
{
    [SerializeField] protected Animator _hitEffectAnimator;

    protected static readonly int _hitEffectHash = Animator.StringToHash("Hit Effect");
    protected static readonly int _explosionEffectHash = Animator.StringToHash("Explosion");

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

    internal abstract void Play();
}

