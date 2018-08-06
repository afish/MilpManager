using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class FactorialCalculator : BaseOperationCalculator
	{
		private static int SoundBoundary(int maximumInteger)
		{
			int size = 2;
			int factorial = 1;
			while (maximumInteger / factorial >= size)
			{
				factorial *= size;
				size ++;
			}

			return size - 1;
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 1 &&
				   arguments.All(a => a.IsInteger() && (a.IsBinary() || a.IsPositiveOrZero()));
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var number = arguments[0];
			var one = milpManager.FromConstant(1);
			var result = one;
			for (int i = SoundBoundary(milpManager.MaximumIntegerValue); i >= 0; --i)
			{
				result = result.Operation<Multiplication>(
					milpManager.Operation<Maximum>(one,
						number.Operation<Subtraction>(milpManager.FromConstant(i))));
			}

			var finalResult = result.ChangeDomain(Domain.PositiveOrZeroInteger);
			SolverUtilities.SetExpression(finalResult, $"{number.FullExpression()}!");
			return finalResult;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var constantArgument = (int)arguments[0].ConstantValue.Value;
			var constantResult = constantArgument == 0 ? 1 : Enumerable.Range(1, constantArgument).Aggregate((a, b) => a * b);
			return milpManager.FromConstant(constantResult);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Factorial)};
	}
}