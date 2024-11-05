using Code.Data;

namespace Code.Services
{
    public interface IAppSettingsService : IService
    {
        public AppSettings Settings { get; set; }
    }
}