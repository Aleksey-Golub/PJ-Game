using UnityEngine;

[CreateAssetMenu(fileName = "new TutorialEventDestroyObject", menuName = "Configs/Tutorial/Tutorial Events/Destroy Object")]
public class TutorialEventDestroyObjectWithId : TutorialEventBase
{
    [field: SerializeField] public string UniqueAssignedId { get; private set; }

    public override void Invoke(Tutorial tutorial)
    {
        tutorial.Handle(this);
    }
}
