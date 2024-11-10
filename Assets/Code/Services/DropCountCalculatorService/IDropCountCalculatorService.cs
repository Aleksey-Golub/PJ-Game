namespace Code.Services
{
    public interface IDropCountCalculatorService : IService
    {
        int Calculate(int originCount, ResourceType resourceType, ToolType needToolType);
    }
}