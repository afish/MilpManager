using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class RemainderCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 && (arguments.All(x => x.IsConstant()) || (arguments.All(x => x.IsPositiveOrZero() || x.IsBinary()) && arguments.All(a => a.IsInteger())));
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			IVariable numerator = arguments[0];
			IVariable denominator = arguments[1];

			var one = milpManager.FromConstant(1);
			var any = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
			any.Operation<Multiplication>(denominator).Set<LessOrEqual>(numerator);
			any.Operation<Addition>(one)
				.Operation<Multiplication>(denominator)
				.Set<GreaterOrEqual>(numerator.Operation<Addition>(one));

			IVariable result = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
			result.Set<LessOrEqual>(denominator);
			result.Set<Equal>(numerator.Operation<Subtraction>(denominator.Operation<Multiplication>(any)));

			result.ConstantValue = numerator.ConstantValue % denominator.ConstantValue;
			result.Expression = $"{numerator.FullExpression()} % {denominator.FullExpression()}";
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var constantResult = (int)arguments[0].ConstantValue.Value % (int)arguments[1].ConstantValue.Value;
			return milpManager.FromConstant(constantResult);
		}

		protected override Type[] SupportedTypes => new [] {typeof(Remainder)};
	}
}