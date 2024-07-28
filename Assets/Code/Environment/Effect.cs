using UnityEngine;

internal class Effect : MonoBehaviour
{
    [SerializeField] private Animator _hitEffectAnimator;
    [SerializeField] private Transform _childEffect;
    [SerializeField] private Vector2 _offset = new Vector2(0, 0.1f);
    [SerializeField] private float _spreadRadius = 0.2f;

    private static readonly int _hitEffectHash = Animator.StringToHash("Hit Effect");

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

    internal void Play()
    {
        _childEffect.localPosition = _offset + Random.insideUnitCircle * _spreadRadius;
        _hitEffectAnimator.Play(_hitEffectHash);
    }
}
