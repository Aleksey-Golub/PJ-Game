namespace Code.Services
{
    public interface IInputService : IService
    {
        void Init();
        float GetHorizontalAxisRaw();
        float GetVerticalAxisRaw();
        bool HasMoveInput();
    }
}