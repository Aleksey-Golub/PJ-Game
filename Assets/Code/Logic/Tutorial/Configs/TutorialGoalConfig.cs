using UnityEngine;

[CreateAssetMenu(fileName = "new TutorialGoal", menuName = "Configs/Tutorial/Tutorial Goal")]
public class TutorialGoalConfig : ScriptableObject
{
    [field: SerializeField] public int RequaredProgress { get; private set; }
    [field: SerializeField] public TutorialGoalType Type { get; private set; }
    [Tooltip("Depends on Type, processed according to type. Can be empty")]
    [field: SerializeField] public string TargetId { get; private set; }
}

public enum TutorialGoalType
{
    None = 0,
    ResourceAdded = 1,
    ResourceRemoved = 2,
    ToolAdded = 3,
    UpgradeTool = 4,
    WaitSomeSeconds = 5,
}