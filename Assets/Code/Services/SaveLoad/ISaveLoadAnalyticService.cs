using Code.Data;

namespace Code.Services
{
    public interface ISaveLoadAnalyticService : IService
    {
        void SaveAnalytic();
        AnalyticData LoadAnalytic();
    }
}