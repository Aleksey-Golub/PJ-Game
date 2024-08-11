using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class PlayerView : MonoBehaviour
{
    [SerializeField] private GameObject _mindCloud;
    [SerializeField] private Image _toolMindImage;
    [SerializeField] private Animator _animator;

    private int _dirXHash;
    private int _dirYHash;
    private int _velocityHash;
    private int _attackHash;

    internal event Action AttackDone;

    internal void Construct()
    {
        _dirXHash = Animator.StringToHash("DirX");
        _dirYHash = Animator.StringToHash("DirY");
        _velocityHash = Animator.StringToHash("Velocity");
        _attackHash = Animator.StringToHash("Attack");
    }

    internal void PlayMove(float dirX, float dirY, float velocity)
    {
        _animator.SetFloat(_dirXHash, dirX);
        _animator.SetFloat(_dirYHash, dirY);
        _animator.SetFloat(_velocityHash, velocity);
    }

    internal void PlayAttack()
    {
        _animator.SetTrigger(_attackHash);
    }

    [UsedImplicitly]
    internal void HitDone()
    {
        //Logger.Log($"HitDone rising {Time.frameCount}");

        AttackDone?.Invoke();
    }

    internal void ShowGatheringBlocked(Sprite sprite)
    {
        if (!_mindCloud.activeSelf)
            _mindCloud.SetActive(true);

        _toolMindImage.sprite = sprite;
    }

    internal void ShowGatheringUnblocked()
    {
        if (_mindCloud.activeSelf)
            _mindCloud.SetActive(false);
    }
}
