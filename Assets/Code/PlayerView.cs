using UnityEngine;

internal class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private int _dirXHash;
    private int _dirYHash;
    private int _velocityHash;
    private int _attackHash;
    
    private void Awake()
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
}
