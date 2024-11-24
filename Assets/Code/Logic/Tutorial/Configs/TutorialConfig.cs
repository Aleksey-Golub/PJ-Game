using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new TutorialConfig", menuName = "Configs/Tutorial/Tutorial Config")]
public class TutorialConfig : ScriptableObject
{
    [field: SerializeField] public List<TutorialStageConfig> Stages { get; private set; }
}
