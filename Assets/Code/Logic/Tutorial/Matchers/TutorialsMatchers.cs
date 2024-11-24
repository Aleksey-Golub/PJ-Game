using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new TutorialsMatchers", menuName = "Configs/Tutorial/Tutorials Matchers")]
public class TutorialsMatchers : ScriptableObject
{
    public List<TutorialMatcher> Matchers;
}
