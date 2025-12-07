using Code.Services;

namespace Code.Infrastructure
{
    public interface IPlayerProvider : IService
    {
        Player GetPlayer();
    }
}