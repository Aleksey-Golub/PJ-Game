using UnityEngine;

[CreateAssetMenu(fileName = "new TutorialEventSpawnObject", menuName = "Configs/Tutorial/Tutorial Events/Spawn Object")]
public class TutorialEventSpawnObjectWithId : TutorialEventBase
{
    [GameObjectIdHolder]
    [SerializeField] private string _spawnedGameObjectId;
    [field: SerializeField] public string AssignedId { get; private set; }
    [field: SerializeField] public Vector3 Position { get; private set; }

    public string SpawnedGameObjectId => _spawnedGameObjectId;

    public override void Invoke(Tutorial tutorial)
    {
        tutorial.Handle(this);
    }
}
