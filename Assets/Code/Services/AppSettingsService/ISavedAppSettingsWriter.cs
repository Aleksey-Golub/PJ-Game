using Code.Data;

namespace Code.Services
{
    public interface ISavedAppSettingsWriter
    {
        void WriteToAppSettings(AppSettings appSettings);
    }
}