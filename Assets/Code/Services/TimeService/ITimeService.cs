namespace Code.Services
{
    public interface ITimeService : IService
    {
        void ResumeTime();
        void StopTime();
    }
}
