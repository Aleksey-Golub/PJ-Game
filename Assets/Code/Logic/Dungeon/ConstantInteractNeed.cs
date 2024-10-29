using UnityEngine;

[CreateAssetMenu(fileName = "newConstant InteractNeed", menuName = "Configs/Interact Needs/Constant Interact Need")]
public class ConstantInteractNeed : InteractNeed
{
    [SerializeField] private bool _result;
    internal override bool CanInteract(Player player) => _result;
}