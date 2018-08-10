using System;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class RemainderCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
		    return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			IVariable numerator = arguments[0];
			IVariable denominator = arguments[1];

            var domain = arguments[0].IsInteger()
                ? arguments[0].IsBinary() ? Domain.BinaryInteger : arguments[0].IsNonNegative() && arguments[1].IsBinary() ? Domain.BinaryInteger : arguments[0].IsNonNegative() ? Domain.PositiveOrZeroInteger : Domain.AnyInteger
                : arguments[0].IsNonNegative() ? Domain.PositiveOrZeroReal : Domain.AnyReal;

		    IVariable result;
		    if (arguments[0].IsBinary() && arguments[1].IsBinary())
		    {
		        result = milpManager.FromConstant(0);
		    }
		    else if (arguments[0].IsInteger() && arguments[0].IsPositiveOrZero() && arguments[1].IsBinary())
		    {
                result = milpManager.FromConstant(0);
            }
		    else
		    {
                result = numerator.Operation<Subtraction>(numerator.Operation<Division>(denominator).Operation<Multiplication>(denominator)).ChangeDomain(domain);
            }
            
		    result.ConstantValue = numerator.ConstantValue % denominator.ConstantValue;

            SolverUtilities.SetExpression(result, $"{numerator.FullExpression()} % {denominator.FullExpression()}");
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
		    var divisionResult = arguments[0].ConstantValue.Value % arguments[1].ConstantValue.Value;

		    if (arguments[0].IsInteger())
		    {
		        return milpManager.FromConstant((int)divisionResult);
            }
		    else
		    {
		        return milpManager.FromConstant(divisionResult);
            }
		}

		protected override Type[] SupportedTypes => new [] {typeof(Remainder)};
	}
}