using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public class MinimizeMaximumCalculator : IGoalCalculator
    {
        public bool SupportsOperation(GoalType type, params IVariable[] arguments)
        {
            return type == GoalType.MinimizeMaximum && arguments.Any();
        }

        public IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.Operation(OperationType.Maximum, arguments);
            }

            var result = milpManager.CreateAnonymous(Domain.AnyReal);
            foreach (var argument in arguments)
            {
                result.Set(ConstraintType.GreaterOrEqual, argument);
            }

            return result.MakeGoal(GoalType.Minimize);
        }
    }
}