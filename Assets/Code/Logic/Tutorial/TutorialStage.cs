using System.Collections.Generic;
using System.Linq;

public class TutorialStage
{
    private readonly TutorialStageConfig _config;
    private readonly List<TutorialGoal> _goals = new();

    public TutorialStage(TutorialStageConfig config)
    {
        _config = config;

        foreach (TutorialGoalConfig gc in config.Goals)
            _goals.Add(new TutorialGoal(gc));
    }

    public int Number => _config.Number;
    public int NextStageNumber => _config.NextStageNumber;
    public bool IsFinal => _config.IsFinal;
    public bool IsComplited => _goals.All(x => x.IsComplited == true);

    internal void RestoreProgress(int[] goalsProgress)
    {
        if (_goals.Count != goalsProgress.Length)
            Logger.LogWarning($"[{nameof(TutorialStage)}] {_config.name}. Config goals count '{_goals.Count}' unequals to goals progress length '{goalsProgress.Length}. Extra goal will initialize with 0 current progress");

        for (int i = 0; i < _goals.Count; i++)
        {
            TutorialGoal g = _goals[i];
            int gp = i < goalsProgress.Length ? goalsProgress[i] : 0;

            g.CurrentProgress = gp;
        }
    }

    internal void Enter(Tutorial tutorial)
    {
        foreach (var e in _config.OnEnterEvents)
        {
            e.Invoke(tutorial);
        }
    }

    internal void OnPlayerInventory_ResourceAdded(ResourceType type, int addedCount)
    {
        var asd = type.ToString();

        foreach (var goal in _goals)
            if (goal.GoalType == TutorialGoalType.ResourceAdded && type.ToString() == goal.TargetId)
                goal.CurrentProgress += addedCount;
    }

    internal void OnPlayerInventory_ResourceRemoved(ResourceType type, int removedCount)
    {
        foreach (var goal in _goals)
            if (goal.GoalType == TutorialGoalType.ResourceRemoved && type.ToString() == goal.TargetId)
                goal.CurrentProgress += removedCount;
    }
    
    internal void OnPlayerInventory_ToolAdded(ToolType type)
    {
        foreach (var goal in _goals)
            if (goal.GoalType == TutorialGoalType.ToolAdded && type.ToString() == goal.TargetId)
                goal.CurrentProgress += 1;
    }
    
    internal void OnUpgradeItemsProgress_Changed(string itemId, int newLevel)
    {
        foreach (var goal in _goals)
            if (goal.GoalType == TutorialGoalType.UpgradeTool && itemId == goal.TargetId)
                goal.CurrentProgress += 1;
    }

    internal void OnOneSecondPassed()
    {
        foreach (var goal in _goals)
            if (goal.GoalType == TutorialGoalType.WaitSomeSeconds)
                goal.CurrentProgress += 1;
    }

    internal int[] GetGoalsProgresses() => _goals.Select(g => g.CurrentProgress).ToArray();
}
