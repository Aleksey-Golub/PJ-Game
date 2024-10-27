using Code.Data;

namespace Code.Services
{
    public interface IPersistentProgressService : IService
    {
        GameProgress Progress { get; set; }
    }
}