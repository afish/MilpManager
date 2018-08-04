using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Implementation.CompositeOperations;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
	public class MultiplicationCalculator : BaseOperationCalculator
	{
		private static bool MultiplyWithMultipleConstants(IVariable[] arguments)
		{
			return arguments.Count(a => a.IsConstant()) > 1;
	    }

	    private static bool MultiplyWithMultipleBinaries(IVariable[] arguments)
	    {
	        return arguments.Count(a => a.IsBinary()) > 1;
	    }

        private static bool MultiplyAtMostOneNonConstant(IVariable[] arguments)
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

		private IVariable MultiplyTwoAnyVariables(IMilpManager milpManager, Domain domain, IVariable[] arguments)
		{
			var binaries = arguments.Where(a => a.IsBinary()).ToArray();
			var nonBinaries = arguments.Where(a => !a.IsBinary()).ToArray();

			if (binaries.Any())
			{
			    IVariable conjuncted = binaries[0];

			    return MultiplyByBinaryDigit(milpManager, nonBinaries[0], conjuncted).ChangeDomain(domain);
			}
            
		     return MultiplyNonBinaryVariables(milpManager, nonBinaries, domain);
	    }

        private IVariable MultiplyNonBinaryVariables(IMilpManager milpManager, IVariable[] nonBinaries, Domain domain)
		{
			var first = nonBinaries[0];
			var second = nonBinaries[1];
		    var mightBeNegatives = !(first.IsNonNegative() && second.IsNonNegative());
			first = MakePositiveIfNeeded(first);
			second = MakePositiveIfNeeded(second);

			var result = MakeLongMultiplication(milpManager, domain, second, first);
			result = FixSign(milpManager, nonBinaries, mightBeNegatives, result);
			result = result.ChangeDomain(domain);

			return result.Operation<Multiplication>(nonBinaries.Skip(2).ToArray());
		}

		private IVariable FixSign(IMilpManager milpManager, IVariable[] nonBinaries, bool mightBeNegatives, IVariable result)
		{
		    var zero = milpManager.FromConstant(0);

            if (mightBeNegatives)
			{
				var sign =
					nonBinaries[0].Operation<IsGreaterOrEqual>(zero)
						.Operation<IsEqual>(nonBinaries[1].Operation<IsGreaterOrEqual>(zero));
				var two = milpManager.FromConstant(2);
				result =
					MultiplyByBinaryDigit(milpManager, result, sign)
						.Operation<Subtraction>(result.Operation<Division>(two))
						.Operation<Multiplication>(two);
			}
			return result;
		}

		private IVariable MakeLongMultiplication(IMilpManager milpManager, Domain domain, IVariable second,
			IVariable first)
		{
		    if (second.IsInteger())
		    {
		        var secondDigits = second.CompositeOperation<UnsignedMagnitudeDecomposition>().ToArray();

		        return MultiplyByDecomposition(milpManager, domain, first, secondDigits, milpManager.FromConstant(0));
            }
		    else
		    {
		        var secondDigits = second.CompositeOperation<UnsignedMagnitudeDecomposition>().ToArray();
		        var secondNonFractionDigits = secondDigits.Take(DecompositionCalculator.GetDigitsCount(milpManager, 2)).ToArray();
		        var secondFractionDigits = secondDigits.Skip(milpManager.IntegerWidth).Reverse().ToArray();

                var nonFractionPart = MultiplyByDecomposition(milpManager, domain, first, secondNonFractionDigits, milpManager.FromConstant(0));
                var fractionPart = MultiplyByDecomposition(milpManager, domain, first, secondFractionDigits, milpManager.FromConstant(0))
                    .Operation<Multiplication>(milpManager.FromConstant(Math.Pow(2, -secondFractionDigits.Length)));

                return nonFractionPart.Operation<Addition>(fractionPart);
            }
		}

	    private IVariable MultiplyByDecomposition(IMilpManager milpManager, Domain domain, IVariable first,
	        IVariable[] secondDigits, IVariable result)
	    {
	        for (int index = 0, power = 1; index < secondDigits.Length; ++index, power = power * 2)
	        {
	            result = result.Operation<Addition>(MultiplyByBinaryDigit(milpManager, first, secondDigits[index])
	                .ChangeDomain(domain)
	                .Operation<Multiplication>(milpManager.FromConstant(power))
	            );
	        }

	        return result;
	    }

	    private static IVariable MakePositiveIfNeeded(IVariable variable)
		{
			if (!variable.IsNonNegative())
			{
				variable = variable.Operation<AbsoluteValue>();
			}

			return variable;
		}

		private IVariable MultiplyByBinaryDigit(IMilpManager milpManager, IVariable number, IVariable digit)
		{
			if (number.Domain == Domain.AnyInteger || number.Domain == Domain.AnyReal)
			{
				var absoluteNumber = number.Operation<AbsoluteValue>();
				var result = MultiplyByBinaryDigit(milpManager, absoluteNumber, digit);
				var two = milpManager.FromConstant(2);
				return MultiplyByBinaryDigit(milpManager, result, number.Operation<IsGreaterOrEqual>(milpManager.FromConstant(0)))
						.Operation<Subtraction>(result.Operation<Division>(two))
						.Operation<Multiplication>(two);
			}

			IVariable digitMultipliedByInfinity = digit.Operation<Multiplication>(milpManager.FromConstant(milpManager.MaximumIntegerValue));

			return milpManager.Operation<Minimum>(
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
			else if (arguments.All(a => a.IsNonNegative()))
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
			return HasVariablesToMultiply(arguments);
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			if (arguments.Length == 1)
			{
				return arguments[0];
			}

			var domain = CalculateDomain(arguments);

			if (MultiplyAtMostOneNonConstant(arguments))
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

		    if (MultiplyWithMultipleConstants(arguments))
		    {
		        var constants = arguments.Where(a => a.IsConstant()).ToArray();
		        var nonConstants = arguments.Where(a => a.IsNotConstant()).ToArray();

		        return milpManager.Operation<Multiplication>(constants).Operation<Multiplication>(nonConstants);
		    }

		    if (MultiplyWithMultipleBinaries(arguments))
		    {
		        var binaries = arguments.Where(a => a.IsBinary()).ToArray();
                IVariable conjuncted = milpManager.Operation<Multiplication>(binaries);
		        var nonBinaries = arguments.Where(a => !a.IsBinary()).ToArray();

                return conjuncted.Operation<Multiplication>(nonBinaries.ToArray());
		    }

            if (arguments.Length > 2)
		    {
		        return arguments[0].Operation<Multiplication>(arguments[1]).Operation<Multiplication>(arguments.Skip(2).ToArray());
            }

		    return MultiplyTwoAnyVariables(milpManager, domain, arguments.Take(2).ToArray());
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
