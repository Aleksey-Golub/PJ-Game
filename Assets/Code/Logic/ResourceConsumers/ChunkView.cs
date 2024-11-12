public class ChunkView : ResourceConsumerView
{
    internal override void ShowNeeds(int currentNeedResourceCount, int totalNeedResourceCount)
    {
        int uploaded = totalNeedResourceCount - currentNeedResourceCount;
        _needText.text = $"{uploaded}/{totalNeedResourceCount}";

        _cloud.SetActive(currentNeedResourceCount != 0);
    }
}

