using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
	public class AdditionCalculator : BaseOperationCalculator
	{
		private static Domain CalculateDomain(IVariable[] arguments)
		{
			if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
			{
				if (arguments.Any(a => a.IsReal()))
				{
					return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroConstantReal;
				}

				return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroConstantInteger;
			}

			if (arguments.Any(a => a.IsReal()))
			{
				return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyReal : Domain.AnyConstantReal;
			}

			return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyInteger : Domain.AnyConstantInteger;
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length > 0;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var domain = CalculateDomain(arguments);

			return arguments.Aggregate((x, y) =>
			{
				var result = milpManager.SumVariables(x, y, domain);
				result.ConstantValue = x.ConstantValue + y.ConstantValue;
				result.Expression = $"{x.FullExpression()} + {y.FullExpression()}";
				return result;
			});
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var sum = arguments.Select(x => x.ConstantValue.Value).Sum();
			if (arguments.All(x => x.IsInteger()))
			{
				return milpManager.FromConstant((int)sum);
			}
			return milpManager.FromConstant(sum);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Addition)};
	}
}
