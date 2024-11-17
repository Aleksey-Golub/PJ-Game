namespace Code.Services
{
    public interface IRestoreState<T>
    {
        void RestoreState(T state);
    }
}