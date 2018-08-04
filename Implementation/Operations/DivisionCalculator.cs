using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
	public class DivisionCalculator : BaseOperationCalculator
	{
		private static Domain CalculateDomain(IVariable[] arguments)
		{
			if (arguments.All(a => a.IsBinary()))
			{
				return Domain.BinaryInteger;
			}

			if (arguments.All(a => a.IsNonNegative()))
			{
			    return Domain.PositiveOrZeroInteger;
			}

		    return Domain.AnyInteger;
		}

		private static bool IsDividingByConstant(IVariable[] arguments)
		{
			return arguments[1].IsConstant();
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 && arguments[1].IsInteger();
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var domain = CalculateDomain(arguments);
			if (IsDividingByConstant(arguments))
			{
				var finalDomain = arguments.All(x => x.IsConstant()) ? domain.MakeConstant() : domain;
				var physicalResult = milpManager.DivideVariableByConstant(arguments[0], arguments[1], finalDomain);
				physicalResult.ConstantValue = arguments[0].ConstantValue / arguments[1].ConstantValue;
				SolverUtilities.SetExpression(physicalResult, $"{arguments[0].FullExpression()} / {arguments[1].FullExpression()}");

				return physicalResult;
			}

			IVariable one = milpManager.FromConstant(1);
			var result = milpManager.CreateAnonymous(domain);

		    result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
		        ? arguments[1].ConstantValue.Value == 0
		            ? (double?)null
		            : (long)arguments[0].ConstantValue.Value / (long)arguments[1].ConstantValue.Value
		        : null;

		    var first = MultiplicationCalculator.MakePositiveIfNeeded(arguments[0]);
		    var second = MultiplicationCalculator.MakePositiveIfNeeded(arguments[1]);

            result.Operation<Multiplication>(second)
				.Set<LessOrEqual>(first);

			result.Operation<Addition>(milpManager.FromConstant(1))
			    .Operation<Multiplication>(second)
				.Set<GreaterOrEqual>(first.Operation<Addition>(one));

		    result = MultiplicationCalculator.FixSign(milpManager, arguments, result);
			SolverUtilities.SetExpression(result, $"{first.FullExpression()} / {second.FullExpression()}");

			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var constantResult = arguments[0].ConstantValue.Value / arguments[1].ConstantValue.Value;
			if (arguments.All(a => a.IsInteger()))
			{
				return milpManager.FromConstant((int)constantResult);
			}
			else
			{
				return milpManager.FromConstant(constantResult);
			}
		}

		protected override Type[] SupportedTypes => new[] {typeof (Division)};
	}
}
