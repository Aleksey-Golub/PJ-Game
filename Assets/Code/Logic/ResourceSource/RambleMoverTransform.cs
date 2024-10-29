using UnityEngine;

internal class RambleMoverTransform : RambleMoverBase
{
    internal override void MoveTo(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            InvokeReached();
        }
    }
}
