using System.Collections;
using UnityEngine;

public class ExhaustStrategy : IExhaustStrategy
{
    private readonly MonoBehaviour _owner;
    private readonly Collider2D _collider;
    private Coroutine _coroutine;

    public ExhaustStrategy(MonoBehaviour owner, Collider2D collider = null)
    {
        _owner = owner;
        _collider = collider;
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
        _owner.gameObject.SetActive(false);
    }

    private void DisableCollider()
    {
        if (_collider != null)
            _collider.enabled = false;
    }
}
