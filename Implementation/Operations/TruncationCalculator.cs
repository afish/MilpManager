using System;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
    public class TruncationCalculator : BaseOperationCalculator
    {
        protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
        {
            return arguments.Length == 1;
        }

        protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
        {
            if (arguments[0].IsInteger())
            {
                return arguments[0];
            }

            var positive = MultiplicationCalculator.MakePositiveIfNeeded(arguments[0]);

            var domain = arguments[0].IsNonNegative() ? Domain.PositiveOrZeroInteger : Domain.AnyInteger;
            var resultPositive = milpManager.CreateAnonymous(domain);
            resultPositive.ConstantValue = arguments[0].ConstantValue.HasValue ? (double?)(int)arguments[0].ConstantValue.Value : null;

            resultPositive.Set<LessOrEqual>(positive);
            resultPositive.Operation<Addition>(milpManager.FromConstant(1)).Set<GreaterThan>(positive);

            var result = MultiplicationCalculator.FixSign(milpManager, arguments, resultPositive).ChangeDomain(domain);
            SolverUtilities.SetExpression(result, $"truncate({arguments[0].FullExpression()})");

            return result;
        }

        protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
        {
            return arguments[0].IsInteger() ? arguments[0] : milpManager.FromConstant((int)arguments[0].ConstantValue.Value);
        }

        protected override Type[] SupportedTypes => new[] { typeof(Truncation) };
    }
}
