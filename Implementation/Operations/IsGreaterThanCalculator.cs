﻿using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class IsGreaterThanCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = milpManager.CreateAnonymous(Domain.BinaryInteger);

			var first = arguments[0];
			var second = arguments[1];

			second.Operation<Subtraction>(first)
				.Operation<Addition>(
					result.Operation<Multiplication>(milpManager.FromConstant(milpManager.IntegerInfinity)))
				.Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(0))
				.Set(ConstraintType.LessOrEqual, milpManager.FromConstant(milpManager.IntegerInfinity - (arguments.Any(a => a.IsReal()) ? milpManager.Epsilon : 1)));

			result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
				? arguments[0].ConstantValue > arguments[1].ConstantValue ? 1 : 0
				: (double?)null;
			result.Expression = $"{arguments[0].FullExpression()} ?> {arguments[1].FullExpression()}";
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments[0].ConstantValue.Value > arguments[1].ConstantValue.Value ? 1 : 0);
		}

		protected override Type[] SupportedTypes => new[] {typeof (IsGreaterThan)};
	}
}
