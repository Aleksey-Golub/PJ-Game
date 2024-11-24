using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new TutorialStage", menuName = "Configs/Tutorial/Tutorial Stage")]
public class TutorialStageConfig : ScriptableObject
{
    [field: SerializeField] public int Number { get; private set; }
    [field: SerializeField] public int NextStageNumber { get; private set; }
    [field: SerializeField] public bool IsFinal { get; private set; }
    [field: SerializeField] public TutorialEventBase[] OnEnterEvents { get; private set; }
    [field: SerializeField] public List<TutorialGoalConfig> Goals { get; private set; }
}
