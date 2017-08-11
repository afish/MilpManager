using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class MaximumMinimumCalculator : BaseOperationCalculator
	{
		private static Domain CalculateDomain(params IVariable[] arguments)
		{
			if (arguments.Any(a => a.IsReal()))
			{
				if (arguments.All(a => a.IsPositiveOrZero()))
				{
					return Domain.PositiveOrZeroReal;
				}
				return Domain.AnyReal;
			}

			if (arguments.All(a => a.IsBinary()))
			{
				return Domain.BinaryInteger;
			}

			if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
			{
				return Domain.PositiveOrZeroInteger;
			}

			return Domain.AnyInteger;
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length >= 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			if (arguments.Length > 2)
			{
				return arguments[0].Operation<TOperationType>(milpManager.Operation<TOperationType>(arguments.Skip(1).ToArray()));
			}
			else
			{
				var first = arguments[0];
				var second = arguments[1];

				IVariable max = milpManager.CreateAnonymous(CalculateDomain(arguments));
				IVariable min = milpManager.CreateAnonymous(CalculateDomain(arguments));

				max.Set<GreaterOrEqual>(first);
				max.Set<GreaterOrEqual>(second);
				min.Set<LessOrEqual>(first);
				min.Set<LessOrEqual>(second);

				max.Operation<Subtraction>(min)
					.Set<Equal>(first.Operation<Subtraction>(second).Operation<AbsoluteValue>());

				max.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
					? Math.Max(arguments[0].ConstantValue.Value, arguments[1].ConstantValue.Value)
					: (double?)null;
				min.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
					? Math.Min(arguments[0].ConstantValue.Value, arguments[1].ConstantValue.Value)
					: (double?)null;
				max.Expression = $"max({arguments[0].FullExpression()}, {arguments[1].FullExpression()}";
				min.Expression = $"min({arguments[0].FullExpression()}, {arguments[1].FullExpression()}";
				return typeof(TOperationType) == typeof(Maximum) ? max : min;
			}
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var values = arguments.Select(a => a.ConstantValue.Value);
			var result = typeof(TOperationType) == typeof(Maximum) ? values.Max() : values.Min();
			if (arguments.All(a => a.IsInteger()))
			{
				return milpManager.FromConstant((int)result);
			}
			else
			{
				return milpManager.FromConstant(result);
			}
		}

		protected override Type[] SupportedTypes => new[] {typeof (Maximum), typeof (Minimum)};
	}
}
