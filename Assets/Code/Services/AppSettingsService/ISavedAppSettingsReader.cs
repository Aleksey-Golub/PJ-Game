using Code.Data;

namespace Code.Services
{
    public interface ISavedAppSettingsReader
    {
        void ReadAppSettings(AppSettings appSettings);
    }
}