using Code.Data;

namespace Code.Services
{
    public interface ISaveLoadIAPDataService : IService
    {
        void SaveIAPData();
        PlayerIAPsData LoadIAPData();
    }
}