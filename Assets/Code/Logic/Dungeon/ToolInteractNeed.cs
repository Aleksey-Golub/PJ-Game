using UnityEngine;

[CreateAssetMenu(fileName = "newTool InteractNeed", menuName = "Configs/Interact Needs/Tool Interact Need")]
public class ToolInteractNeed : InteractNeed
{
    [SerializeField] private ToolType _toolType;

    internal override bool CanInteract(Player player)
    {
        bool hasTool = player.Has(_toolType);

        if (!hasTool)
            player.SetLastAbsentTool(_toolType);

        return hasTool;
    }
}
