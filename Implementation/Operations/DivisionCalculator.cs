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

			if (IsDividingByConstant(arguments))
			{
				if (arguments.All(a => a.IsInteger()))
				{

					if (arguments.All(a => a.IsNonNegative()))
					{
						return Domain.PositiveOrZeroInteger;
					}
					return Domain.AnyInteger;
				}
				else
				{
					if (arguments.All(a => a.IsNonNegative()))
					{
						return Domain.PositiveOrZeroReal;
					}
					return Domain.AnyReal;
				}
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
			return arguments.Length == 2 &&
				   (IsDividingByConstant(arguments) || arguments.All(a => a.IsNonNegative() && a.IsInteger()));
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

            result.Operation<Multiplication>(arguments[1])
				.Set<LessOrEqual>(arguments[0]);

			result.Operation<Addition>(milpManager.FromConstant(1))
			    .Operation<Multiplication>(arguments[1])
				.Set<GreaterOrEqual>(arguments[0].Operation<Addition>(one));
			SolverUtilities.SetExpression(result, $"{arguments[0].FullExpression()} / {arguments[1].FullExpression()}");
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
