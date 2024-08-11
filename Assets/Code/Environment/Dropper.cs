﻿using System;
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

    public DropData(float moveAfterDropTime, Vector3 finalPosition)
    {
        MoveAfterDropTime = moveAfterDropTime;
        FinalPosition = finalPosition;
    }

    internal static List<DropData> Get(Vector3 originePosition, DropSettings dropSettings, int count = 1)
    {
        List<DropData> result = new(count);

        switch (dropSettings.DropStrategy)
        {
            case DropStrategy.RandomInsideCircle:
                
                for (int i = 0; i < count; i++)
                {
                    Vector3 offset = UnityEngine.Random.insideUnitCircle * dropSettings.DropRadius;
                    offset.z = originePosition.z;

                    Vector3 finalPosition = originePosition + offset;

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition);
                    result.Add(newDropData);
                }
                return result;

            case DropStrategy.SamePosition:

                for (int i = 0; i < count; i++)
                {
                    Vector3 finalPosition = originePosition;

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition);
                    result.Add(newDropData);
                }
                return result;

            case DropStrategy.RadialByCircle:

                Vector3 offsetStart = UnityEngine.Random.insideUnitCircle.normalized * dropSettings.DropRadius;
                offsetStart.z = originePosition.z;
                float step = 360f / count;

                for (int i = 0; i < count; i++)
                {
                    Vector3 offset1 = Quaternion.AngleAxis(step * i, Vector3.forward) * offsetStart;
                    Vector3 finalPosition = originePosition + offset1;

                    DropData newDropData = new DropData(dropSettings.MoveAfterDropTime, finalPosition);
                    result.Add(newDropData);
                }
                return result;

            default:
                throw new NotImplementedException();
        }
    }
}

[System.Serializable]
public struct DropSettings
{
    public DropStrategy DropStrategy;
    public float DropRadius;
    public float MoveAfterDropTime;
}

public enum DropStrategy
{
    RandomInsideCircle = 1,
    SamePosition = 2,
    RadialByCircle = 3,
}

