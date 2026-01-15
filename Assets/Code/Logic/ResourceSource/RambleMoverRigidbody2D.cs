using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class RambleMoverRigidbody2D : RambleMoverBase
{
    [SerializeField] private CastParams _castParams;
    [SerializeField] private Rigidbody2D _rb;

    private readonly RaycastHit2D[] _buffer = new RaycastHit2D[5];
    private readonly Collider2D[] _endBuffer = new Collider2D[5];

    internal override bool IsValid(Vector3 targetPoint)
    {
        Vector3 to = targetPoint - transform.position;
        Vector2 direction = to.normalized;

        int detectCount = Physics2D.CircleCastNonAlloc(transform.position + _castParams.Offset, _castParams.Radius * 0.95f, direction, _buffer, to.magnitude);
        int endPointDetectCount = Physics2D.OverlapCircleNonAlloc(targetPoint, _castParams.Radius * 1.05f, _endBuffer);

        return
            (1 == detectCount || DetectSelfAndTriggersOnly()) // path is clear or detect only self and other triggers
            && (0 == endPointDetectCount || EndPointDetectSelfAndTriggersOnly()); // end point is clear or detect only self and other triggers

        bool DetectSelfAndTriggersOnly()
        {
            foreach (RaycastHit2D item in _buffer)
            {
                if (item.transform != null && item.transform.gameObject != gameObject && !item.collider.isTrigger)
                    return false;
            }

            return true;
        }
        
        bool EndPointDetectSelfAndTriggersOnly()
        {
            foreach (Collider2D collider in _endBuffer)
            {
                if (collider != null && collider.transform.gameObject != gameObject && !collider.isTrigger)
                    return false;
            }

            return true;
        }
    }

    internal override void MoveTo(Vector3 targetPosition)
    {
        Vector2 startPos = _rb.position;
        Vector2 endPos = targetPosition;

        if (startPos == endPos)
        {
            InvokeReached();
            return;
        }

        Vector3 to = endPos - startPos;
        Vector2 direction = to.normalized;
        Vector2 offset = direction * Speed * Time.fixedDeltaTime;

        if (offset.sqrMagnitude < to.sqrMagnitude)
        {
            _rb.MovePosition(startPos + offset);
        }
        else
        {
            _rb.MovePosition(endPos);
        }
    }

    [System.Serializable]
    public struct CastParams
    {
        public Vector3 Offset;
        public float Radius;
    }
}
