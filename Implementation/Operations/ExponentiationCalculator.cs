using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class ExponentiationCalculator : BaseOperationCalculator
	{
		private static IVariable CalculatePower(IVariable number, IVariable power, IMilpManager milpManager, IVariable isEdgeCase)
		{
			var digits = (int)Math.Ceiling(Math.Log(milpManager.IntegerWidth, 2.0));

			var infinity = milpManager.FromConstant(milpManager.MaximumIntegerValue);
			var currentPower = milpManager.Operation<Minimum>(number, isEdgeCase.Operation<BinaryNegation>().Operation<Multiplication>(infinity));
			var decomposition = power.CompositeOperation<UnsignedMagnitudeDecomposition>().Take(digits).ToArray();
			var one = milpManager.FromConstant(1);
			var result = one;

			for (int i = 0; i < digits; ++i)
			{
				if (i > 0)
				{
					var isAnyNonzeroDigitLater = milpManager.Operation<Disjunction>(decomposition.Skip(i).ToArray());
					var numberToMultiply = milpManager.Operation<Minimum>(currentPower, isAnyNonzeroDigitLater.Operation<Multiplication>(infinity));
					currentPower = numberToMultiply.Operation<Multiplication>(numberToMultiply);
				}

				result = result.Operation<Multiplication>(one.Operation<Maximum>(currentPower.Operation<Multiplication>(decomposition[i])));
			}

			return result;
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 && (arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger()) || arguments.All(a => a.IsConstant()));
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var number = arguments[0];
			var power = arguments[1];

			var one = milpManager.FromConstant(1);
			var zero = milpManager.FromConstant(0);
			var isNumberLessOrEqualOne = number.Operation<IsLessOrEqual>(one);
			var isPowerZero = power.Operation<IsLessOrEqual>(zero);
			var isPowerOne = power.Operation<IsEqual>(one);
			var isEdgeCase = milpManager.Operation<Disjunction>(isNumberLessOrEqualOne, isPowerZero, isPowerOne);
			var result = milpManager.Operation<Condition>(
				isPowerZero,
				one,
				milpManager.Operation<Condition>(
					isNumberLessOrEqualOne,
					number,
					milpManager.Operation<Condition>(
						isPowerOne,
						number,
						CalculatePower(number, power, milpManager, isEdgeCase)
					)
				)
			);

			result.ConstantValue = number.ConstantValue.HasValue && power.ConstantValue.HasValue
				? number.ConstantValue == 0
					? 0.0
					: power.ConstantValue == 0
						? number.ConstantValue
						: Math.Pow(number.ConstantValue.Value, power.ConstantValue.Value)
				: (double?)null;
			SolverUtilities.SetExpression(result, $"{number.FullExpression()} ** {power.FullExpression()}");
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var constantResult = Math.Pow(arguments[0].ConstantValue.Value, arguments[1].ConstantValue.Value);
			if (arguments.All(a => a.IsInteger()))
			{
				return milpManager.FromConstant((int)constantResult);
			}
			else
			{
				return milpManager.FromConstant(constantResult);
			}
		}

		protected override Type[] SupportedTypes => new[] {typeof (Exponentiation)};
	}
}