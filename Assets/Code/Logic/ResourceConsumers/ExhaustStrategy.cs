using System;
using System.Collections;
using UnityEngine;

public class ExhaustStrategy : IExhaustStrategy
{
    private readonly MonoBehaviour _owner;
    private readonly Collider2D _collider;
    private readonly Action _onExhaused;
    private readonly bool _disableSelf;
    private Coroutine _coroutine;

    public ExhaustStrategy(MonoBehaviour owner, Collider2D disablingCollider = null, System.Action onExhaused = null, bool disableSelf = true)
    {
        _owner = owner;
        _collider = disablingCollider;
        _onExhaused = onExhaused;
        _disableSelf = disableSelf;
    }

    public void ExhaustDelayed(float delay)
    {
        if (_coroutine != null)
            _owner.StopCoroutine(_coroutine);

        _coroutine = _owner.StartCoroutine(OnExhaustCor(delay));
    }

    public void ExhaustImmediately()
    {
        DisableCollider();
        InactivateSelf();
    }

    private IEnumerator OnExhaustCor(float delay)
    {
        WaitForSeconds waitDelay = new WaitForSeconds(delay);

        yield return waitDelay;
        DisableCollider();
        yield return waitDelay;
        InactivateSelf();
    }

    private void InactivateSelf()
    {
        _onExhaused?.Invoke();

        if (_disableSelf)
            _owner?.gameObject.SetActive(false);
    }

    private void DisableCollider()
    {
        if (_collider != null)
            _collider.enabled = false;
    }
}
