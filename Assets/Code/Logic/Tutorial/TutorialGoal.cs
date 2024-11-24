public class TutorialGoal
{
    private readonly TutorialGoalConfig _config;

    public TutorialGoal(TutorialGoalConfig config)
    {
        _config = config;
    }

    internal TutorialGoalType GoalType => _config.Type;
    internal string TargetId => _config.TargetId;
    internal int CurrentProgress { get; set; }
    internal int RequaredProgress => _config.RequaredProgress;
    internal bool IsComplited => CurrentProgress >= RequaredProgress;
}
