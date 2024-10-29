using System.Collections.Generic;
using UnityEngine;

internal class ResourceSourceHitByHitGatheringView : ResourceSourceViewBase
{
    [Header("Self")]
    [SerializeField] private List<GameObject> _hpBarObjs;

    internal override void ShowHP(int currentHitPoints, int totalHitPoints)
    {
        for (int i = 0; i < _hpBarObjs.Count; i++)
        {
            _hpBarObjs[i].SetActive(i < currentHitPoints);
        }
    }
}

