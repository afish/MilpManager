using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class AbsoluteValueCalculator : BaseOperationCalculator
	{

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 1;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var number = arguments[0];
			var numberNegated = number.Operation<Negation>();
			var result = milpManager.CreateAnonymous(number.IsInteger() ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroReal);

			result.Set(ConstraintType.GreaterOrEqual, number)
				.Set(ConstraintType.GreaterOrEqual, numberNegated);

			milpManager.Operation<Addition>(
					result.Operation<IsEqual>(number),
					result.Operation<IsEqual>(numberNegated))
				.Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(1));

			result.ConstantValue = number.ConstantValue.HasValue ? Math.Abs(number.ConstantValue.Value) : number.ConstantValue;
			result.Expression = $"|{number.FullExpression()}|";
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			if (arguments[0].IsInteger())
			{
				return milpManager.FromConstant(Math.Abs((int)arguments[0].ConstantValue.Value));
			}
			return milpManager.FromConstant(Math.Abs(arguments[0].ConstantValue.Value));
		}

		protected override Type[] SupportedTypes => new[] {typeof (AbsoluteValue)};
	}
}
