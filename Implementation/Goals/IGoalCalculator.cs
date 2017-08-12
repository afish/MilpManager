using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public interface  IGoalCalculator
    {
        bool SupportsOperation<TGoalType>(params IVariable[] arguments) where TGoalType : GoalType;

        IVariable Calculate<TGoalType>(IMilpManager milpManager, params IVariable[] arguments) where TGoalType : GoalType;
    }
}