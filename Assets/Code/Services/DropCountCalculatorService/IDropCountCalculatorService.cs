namespace Code.Services
{
    internal interface IDropCountCalculatorService : IService
    {
        int Calculate(int originCount, ResourceType resourceType, ToolType needToolType);
    }
}