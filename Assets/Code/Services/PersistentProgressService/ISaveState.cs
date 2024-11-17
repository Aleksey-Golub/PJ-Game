namespace Code.Services
{
    public interface ISaveState<T>
    {
        T SaveState();
    }
}