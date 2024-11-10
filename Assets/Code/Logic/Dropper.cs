using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Dropper
{
    private Coroutine _moveAfterDropCoroutine;

    internal void MoveAfterDrop(MonoBehaviour drop, DropObjectViewBase view, Collider2D collider, DropData dropData)
    {
        if (_moveAfterDropCoroutine != null)
            drop.StopCoroutine(_moveAfterDropCoroutine);

        _moveAfterDropCoroutine = drop.StartCoroutine(MoveAfterDropCor(drop.transform, view, collider, dropData));
    }

    private IEnumerator MoveAfterDropCor(Transform transform, DropObjectViewBase view, Collider2D collider, DropData dropData)
    {
        view.ShowStartDrop();

        Vector3 finalPosition = dropData.FinalPosition;
        Vector3 startPosition = transform.position;
        float timer = 0;
        var moveAfterDropTime = dropData.MoveAfterDropTime;

        while (timer < moveAfterDropTime)
        {
            float t = timer / moveAfterDropTime;
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

internal struct DropData
{
    internal float MoveAfterDropTime;
    internal Vector3 FinalPosition;
    internal int ResourceInPackCount;

    public DropData(float moveAfterDropTime, Vector3 finalPosition, int resourceInPackCount)
    {
        MoveAfterDropTime = moveAfterDropTime;
        FinalPosition = finalPosition;
        ResourceInPackCount = resourceInPackCount;
    }

    internal static List<DropData> Get(Vector3 originePosition, DropSettings dropSettings, int count, out int notFittedCount)
    {
        int packsCount = dropSettings.DropGroupingStrategy == DropGroupingStrategy.Individual ? count : 1;
        int countInPack = count / packsCount;
        notFittedCount = count % packsCount;

        List<DropData> result = new(packsCount);

        switch (dropSettings.DropStrategy)
        {
            case DropStrategy.RandomInsideCircle:

                for (int i = 0; i < packsCount; i++)
                {
                    Vector3 finalPosition = GetRandomInsideCirclePosition(originePosition, dropSettings);

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition, countInPack);
                    result.Add(newDropData);
                }
                return result;

            case DropStrategy.SamePosition:

                for (int i = 0; i < packsCount; i++)
                {
                    Vector3 finalPosition = originePosition;

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition, countInPack);
                    result.Add(newDropData);
                }
                return result;

            case DropStrategy.RadialByCircle:

                Vector3 offsetStart = UnityEngine.Random.insideUnitCircle.normalized * dropSettings.DropRadius;
                offsetStart.z = originePosition.z;
                float step = 360f / packsCount;

                for (int i = 0; i < packsCount; i++)
                {
                    Vector3 offset1 = Quaternion.AngleAxis(step * i, Vector3.forward) * offsetStart;
                    Vector3 finalPosition = originePosition + offset1;
                    finalPosition = IsValid(finalPosition) ? finalPosition : originePosition;

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition, countInPack);
                    result.Add(newDropData);
                }
                return result;

            default:
                throw new NotImplementedException();
        }
    }

    private static Vector3 GetRandomInsideCirclePosition(Vector3 originePosition, DropSettings dropSettings)
    {
        Vector3 finalPosition;
        int @try = 0;
        int maxTryCount = 5;

        do
        {
            @try++;

            if (@try > maxTryCount)
            {
                finalPosition = originePosition;
                break;
            }

            Vector3 offset = UnityEngine.Random.insideUnitCircle * dropSettings.DropRadius;
            offset.z = originePosition.z;

            finalPosition = originePosition + offset;

        } while (!IsValid(finalPosition));

        return finalPosition;
    }

    private static bool IsValid(Vector3 finalPosition)
    {
        const float checkRadius = 0.1f;

        foreach (Collider2D c in Physics2D.OverlapCircleAll(finalPosition, checkRadius))
            if (!c.isTrigger)
                return false;

        return true;
    }
}

[System.Serializable]
public struct DropSettings
{
    public DropStrategy DropStrategy;
    public DropGroupingStrategy DropGroupingStrategy;
    public float DropRadius;
    public float MoveAfterDropTime;

    public static DropSettings Default = new()
    {
        DropRadius = 1.3f,
        MoveAfterDropTime = 0.6f,
        DropStrategy = DropStrategy.RandomInsideCircle,
        DropGroupingStrategy = DropGroupingStrategy.AllTogether
    };

    public void DrawRadius(Vector3 position, Color color = default)
    {
        if (color == default)
            color = Color.gray;

        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, DropRadius);
    }
}

public enum DropStrategy
{
    RandomInsideCircle = 1,
    SamePosition = 2,
    RadialByCircle = 3,
}

public enum DropGroupingStrategy
{
    Individual = 1,
    AllTogether = 2,
}

