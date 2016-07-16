using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public interface  IGoalCalculator
    {
        bool SupportsOperation(GoalType type, params IVariable[] arguments);

        IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments);
    }
}