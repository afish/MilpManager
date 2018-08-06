using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class GcdCalculator : BaseOperationCalculator
	{
		private static IVariable CalculateInternal(IMilpManager milpManager, params IVariable[] arguments)
		{
			var a = arguments[0];
			var b = arguments[1];

			var gcd = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
		    gcd.ConstantValue = a.ConstantValue.HasValue && b.ConstantValue.HasValue
		        ? Gcd((int)a.ConstantValue.Value, (int)b.ConstantValue.Value)
		        : (double?)null;

            var x = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
			var y = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
			var m = milpManager.CreateAnonymous(Domain.AnyInteger);
			var n = milpManager.CreateAnonymous(Domain.AnyInteger);

			gcd.Set<GreaterOrEqual>(milpManager.FromConstant(1));
			a.Set<Equal>(x.Operation<Multiplication>(gcd));
			b.Set<Equal>(y.Operation<Multiplication>(gcd));
			gcd.Set<Equal>(m.Operation<Multiplication>(a).Operation<Addition>(n.Operation<Multiplication>(b)));

			SolverUtilities.SetExpression(gcd, $"gcd({a.FullExpression()}, {b.FullExpression()})");
			return gcd;
		}
		private static int Gcd(int a, int b)
		{
			return b == 0 ? a : Gcd(b, a % b);
		}

		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 &&
				   arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var a = arguments[0];
			var b = arguments[1];
			var gcd = CalculateInternal(milpManager, a, b);

			return gcd;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(Gcd((int)arguments[0].ConstantValue.Value, (int)arguments[1].ConstantValue.Value));
		}

		protected override Type[] SupportedTypes => new[] {typeof (GCD)};
	}
}