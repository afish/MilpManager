using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public class MaximizeMinimumCalculator : IGoalCalculator
    {
        public bool SupportsOperation(GoalType type, params IVariable[] arguments)
        {
            return type == GoalType.MaximizeMinium && arguments.Any();
        }

        public IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.Operation<Minimum>(arguments);
            }

            var result = milpManager.CreateAnonymous(arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger);
            foreach (var argument in arguments)
            {
                result.Set(ConstraintType.LessOrEqual, argument);
            }

            return result;
        }
    }
}