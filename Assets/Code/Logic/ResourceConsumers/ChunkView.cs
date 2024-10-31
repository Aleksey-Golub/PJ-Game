internal class ChunkView : ResourceConsumerView
{
    internal void ShowNeeds(int currentNeedResourceCount, int totalNeedResourceCount)
    {
        int uploaded = totalNeedResourceCount - currentNeedResourceCount;
        _needText.text = $"{uploaded}/{totalNeedResourceCount}";

        _cloud.SetActive(currentNeedResourceCount != 0);
    }
}

