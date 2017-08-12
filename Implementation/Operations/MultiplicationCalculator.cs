using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
	public class MultiplicationCalculator : BaseOperationCalculator
	{
		private static bool MultiplyAnyIntegers(IVariable[] arguments)
		{
			return arguments.All(a => a.IsInteger());
		}

		private static bool MultiplyOnlyConstants(IVariable[] arguments)
		{
			return arguments.All(a => a.IsConstant());
		}

		private static bool MultiplyAtMostOneNonconstant(IVariable[] arguments)
		{
			return arguments.Count(a => a.IsNotConstant()) <= 1;
		}

		private static bool MultiplyBinaryVariables(IVariable[] arguments)
		{
			return arguments.All(a => a.IsBinary());
		}

		private static bool HasVariablesToMultiply(IVariable[] arguments)
		{
			return arguments.Length >= 1;
		}

		private IVariable MultiplyIntegers(IMilpManager baseMilpManager, Domain domain, IVariable[] arguments)
		{
			var binaries = arguments.Where(a => a.IsBinary()).ToArray();
			var nonBinaries = arguments.Where(a => !a.IsBinary()).ToArray();

			if (binaries.Any())
			{
				IVariable conjucted = baseMilpManager.Operation<Multiplication>(binaries);
				return MultipleByBinaryDigit(baseMilpManager, nonBinaries[0], conjucted).ChangeDomain(domain)
					.Operation<Multiplication>(nonBinaries.Skip(1).ToArray());
			}

			return MultiplyNonBinaryIntegers(baseMilpManager, nonBinaries, domain);
		}

		private IVariable MultiplyNonBinaryIntegers(IMilpManager baseMilpManager, IVariable[] nonBinaries, Domain domain)
		{

			var first = nonBinaries[0];
			var second = nonBinaries[1];
			var mightBeNegatives = first.Domain == Domain.AnyInteger || first.Domain == Domain.AnyConstantInteger ||
					 second.Domain == Domain.AnyInteger || second.Domain == Domain.AnyConstantInteger;
			first = MakePositiveIfNeeded(first);
			second = MakePositiveIfNeeded(second);

			var zero = baseMilpManager.FromConstant(0);
			var result = MakeLongMultiplication(baseMilpManager, domain, zero, second, first);
			result = FixSign(baseMilpManager, nonBinaries, mightBeNegatives, zero, result);
			result = result.ChangeDomain(domain);

			return result.Operation<Multiplication>(nonBinaries.Skip(2).ToArray());
		}

		private IVariable FixSign(IMilpManager baseMilpManager, IVariable[] nonBinaries, bool mightBeNegatives, IVariable zero,
			IVariable result)
		{
			if (mightBeNegatives)
			{
				var sign =
					nonBinaries[0].Operation<IsGreaterOrEqual>(zero)
						.Operation<IsEqual>(nonBinaries[1].Operation<IsGreaterOrEqual>(zero));
				var two = baseMilpManager.FromConstant(2);
				result =
					MultipleByBinaryDigit(baseMilpManager, result, sign)
						.Operation<Subtraction>(result.Operation<Division>(two))
						.Operation<Multiplication>(two);
			}
			return result;
		}

		private IVariable MakeLongMultiplication(IMilpManager baseMilpManager, Domain domain, IVariable zero, IVariable second,
			IVariable first)
		{
			var result = zero;

			var secondDigits = second.CompositeOperation<UnsignedMagnitudeDecomposition>().ToArray();
			for (int index = 0, power = 1; index < secondDigits.Length; ++index, power = power*2)
			{
				result = result.Operation<Addition>(MultipleByBinaryDigit(baseMilpManager, first, secondDigits[index])
						.ChangeDomain(domain)
						.Operation<Multiplication>(baseMilpManager.FromConstant(power))
					);
			}
			return result;
		}

		private static IVariable MakePositiveIfNeeded(IVariable variable)
		{
			if (variable.Domain == Domain.AnyInteger || variable.Domain == Domain.AnyConstantInteger)
			{
				variable = variable.Operation<AbsoluteValue>();
			}

			return variable;
		}

		private IVariable MultipleByBinaryDigit(IMilpManager baseMilpManager, IVariable number, IVariable digit)
		{
			if (number.Domain == Domain.AnyConstantInteger || number.Domain == Domain.AnyInteger)
			{
				var absoluteNumber = number.Operation<AbsoluteValue>();
				var result = MultipleByBinaryDigit(baseMilpManager, absoluteNumber, digit);
				var two = baseMilpManager.FromConstant(2);
				return MultipleByBinaryDigit(baseMilpManager, result, number.Operation<IsGreaterOrEqual>(baseMilpManager.FromConstant(0)))
						.Operation<Subtraction>(result.Operation<Division>(two))
						.Operation<Multiplication>(two);
			}

			IVariable digitMultipliedByInfinity = digit.Operation<Multiplication>(baseMilpManager.FromConstant(baseMilpManager.MaximumIntegerValue));
			return baseMilpManager.Operation<Minimum>(
				number,
				digitMultipliedByInfinity
				);
		}

		private static Domain CalculateDomain(IVariable[] arguments)
		{
			Domain domain;
			if (MultiplyBinaryVariables(arguments))
			{
				domain = Domain.BinaryInteger;
			}
			else if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
			{
				domain = arguments.Any(a => a.IsReal()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroInteger;
			}
			else
			{
				domain = arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger;
			}

			return arguments.All(a => a.IsConstant()) ? domain.MakeConstant() : domain;
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return HasVariablesToMultiply(arguments) && (
				MultiplyOnlyConstants(arguments) ||
				MultiplyBinaryVariables(arguments) ||
				MultiplyAtMostOneNonconstant(arguments) ||
				MultiplyAnyIntegers(arguments));
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			if (arguments.Length == 1)
			{
				return arguments[0];
			}

			var domain = CalculateDomain(arguments);

			if (MultiplyAtMostOneNonconstant(arguments))
			{
				return arguments.Aggregate((x, y) =>
				{
					var result = y.IsConstant()
						? milpManager.MultiplyVariableByConstant(x, y, domain)
						: milpManager.MultiplyVariableByConstant(y, x, domain);
					result.ConstantValue = x.ConstantValue * y.ConstantValue;
					SolverUtilities.SetExpression(result, $"{x.FullExpression()} * {y.FullExpression()}");
					return result;
				});
			}

			if (MultiplyBinaryVariables(arguments))
			{
				return milpManager.Operation<Conjunction>(arguments);
			}

			return MultiplyIntegers(milpManager, domain, arguments);
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = arguments.Select(a => a.ConstantValue.Value).Aggregate((a, b) => a * b);
			if (arguments.All(a => a.IsInteger()))
			{
				return milpManager.FromConstant((int)result);
			}
			else
			{
				return milpManager.FromConstant(result);
			}
		}

		protected override Type[] SupportedTypes => new[] {typeof (Multiplication)};
	}
}
