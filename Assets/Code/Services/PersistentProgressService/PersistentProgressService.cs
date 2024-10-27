using Code.Data;

namespace Code.Services
{
    public class PersistentProgressService : IPersistentProgressService
    {
        public GameProgress Progress { get; set; }
    }
}