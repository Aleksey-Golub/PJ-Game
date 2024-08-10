using System.Collections;
using UnityEngine;

[System.Serializable]
internal class Dropper
{
    [Header("Settings")]
    [SerializeField] private float _dropRadius = 1.3f;
    [SerializeField] private float _moveAfterDropTime = 0.6f;

    private Coroutine _moveAfterDropCoroutine;

    internal void MoveAfterDrop(MonoBehaviour drop, DropObjectViewBase view, Collider2D collider)
    {
        if (_moveAfterDropCoroutine != null)
            drop.StopCoroutine(_moveAfterDropCoroutine);

        _moveAfterDropCoroutine = drop.StartCoroutine(MoveAfterDropCor(drop.transform, view, collider));
    }

    private IEnumerator MoveAfterDropCor(Transform transform, DropObjectViewBase view, Collider2D collider)
    {
        view.ShowStartDrop();
        Vector3 finalPosition = Random.insideUnitCircle * _dropRadius + new Vector2(transform.position.x, transform.position.y);

        finalPosition.z = transform.position.z;
        Vector3 startPosition = transform.position;
        float timer = 0;

        while (timer < _moveAfterDropTime)
        {
            float t = timer / _moveAfterDropTime;
            transform.position = Vector3.Slerp(startPosition, finalPosition, t);
            view.ShowDropping(t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;
        collider.enabled = true;
        view.ShowEndDrop();
    }
}
