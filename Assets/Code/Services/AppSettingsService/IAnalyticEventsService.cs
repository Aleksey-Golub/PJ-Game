using Code.Data;

namespace Code.Services
{
    public interface IAnalyticEventsService : IService
    {
        public AnalyticData AnalyticData { get; set; }
    }
}