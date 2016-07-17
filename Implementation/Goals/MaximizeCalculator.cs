using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public class MaximizeCalculator : IGoalCalculator
    {
        public bool SupportsOperation(GoalType type, params IVariable[] arguments)
        {
            return type == GoalType.Maximize && arguments.Length == 1;
        }

        public IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            return arguments[0];
        }
    }
}