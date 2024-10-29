using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class RambleMoverRigidbody2D : RambleMoverBase
{
    [SerializeField] private Rigidbody2D _rb;

    internal override void MoveTo(Vector3 targetPosition)
    {
        Vector2 startPos = _rb.position;
        Vector2 endPos = new Vector2(targetPosition.x, targetPosition.y);

        if (startPos == endPos)
        {
            print("startPos == endPos");
            InvokeReached();
            return;
        }

        Vector3 to = endPos - startPos;
        Vector2 direction = to.normalized;
        Vector2 offset = direction * Speed * Time.fixedDeltaTime;

        if (offset.sqrMagnitude > to.sqrMagnitude)
        {

            print("move to offset");
            _rb.MovePosition(startPos + offset);
        }
        else
        {
            print("move to end");

            _rb.MovePosition(endPos);
        }
    }
}
