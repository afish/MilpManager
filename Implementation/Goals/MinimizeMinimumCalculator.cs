using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public class MinimizeMinimumCalculator : IGoalCalculator
    {
        public bool SupportsOperation(GoalType type, params IVariable[] arguments)
        {
            return type == GoalType.MinimizeMinimum && arguments.Any();
        }

        public IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.Operation<Minimum>(arguments);
            }
            
            return milpManager.MakeGoal(GoalType.MaximizeMaximum, arguments).MakeGoal(GoalType.Minimize);
        }
    }
}