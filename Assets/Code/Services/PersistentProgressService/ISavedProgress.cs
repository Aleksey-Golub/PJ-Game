using Code.Data;

namespace Code.Services
{
    public interface ISavedProgressWriter
    {
        void WriteToProgress(GameProgress progress);
    }
}