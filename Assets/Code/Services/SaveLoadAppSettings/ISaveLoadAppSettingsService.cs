using Code.Data;

namespace Code.Services
{
    public interface ISaveLoadAppSettingsService : IService
    {
        void SaveAppSettings();
        AppSettings LoadAppSettings();
    }
}