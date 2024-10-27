using Code.Data;

namespace Code.Services
{
  public interface ISavedProgress : ISavedProgressReader
  {
    void UpdateProgress(GameProgress progress);
  }
}