public class ChunkView : ResourceConsumerView
{
    internal override void ShowNeeds(int currentNeedResourceCount, int totalNeedResourceCount, bool isAvailable)
    {
        int uploaded = totalNeedResourceCount - currentNeedResourceCount;
        _needText.text = $"{uploaded}/{totalNeedResourceCount}";

        _cloud.SetActive(isAvailable && currentNeedResourceCount != 0);
    }
}

