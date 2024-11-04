using Code.Data;

namespace Code.Services
{
    public interface ISavedProgressReader
    {
        void ReadProgress(GameProgress progress);
    }
}