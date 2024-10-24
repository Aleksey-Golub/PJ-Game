using UnityEngine;

internal interface IMergingResource
{
    Vector3 Position { get; }
    int Count { get; }
    ResourceType Type { get; }

    bool IsReadyToMerge(float timeToMerge);
    void MoveAfterMerge(Vector3 toPosition);
    void SetCount(int value);
    void UpdateDroppedTime(float deltaTime);
}
