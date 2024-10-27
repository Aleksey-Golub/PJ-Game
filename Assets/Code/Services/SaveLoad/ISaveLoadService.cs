using Code.Data;

namespace Code.Services
{
  public interface ISaveLoadService : IService
  {
    void SaveProgress();
    GameProgress LoadProgress();
  }
}