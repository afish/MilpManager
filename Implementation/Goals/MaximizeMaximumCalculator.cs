using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
    public class MaximizeMaximumCalculator : IGoalCalculator
    {
        public bool SupportsOperation(GoalType type, params IVariable[] arguments)
        {
            return type == GoalType.MaximizeMaximum && arguments.Any();
        }

        public IVariable Calculate(IMilpManager milpManager, GoalType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.Operation<Maximum>(arguments);
            }

            var result = milpManager.CreateAnonymous(arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger);
            arguments.Aggregate(milpManager.FromConstant(0),(existing, next) => existing.Operation<Disjunction>(result.Operation<IsEqual>(next)))
                .Set(ConstraintType.Equal, milpManager.FromConstant(1));

            return result;
        }
    }
}