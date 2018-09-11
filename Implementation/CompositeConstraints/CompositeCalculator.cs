using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeConstraints
{
	public class CompositeCalculator : ICompositeConstraintCalculator
	{
		public IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters,
			IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraint

		{
		    var target = leftVariable.IsNonNegative() ? leftVariable : leftVariable.Operation<AbsoluteValue>();

		    var two = milpManager.FromConstant(2);
            var first = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger).Set<GreaterOrEqual>(two);
		    var second = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger).Set<GreaterOrEqual>(two);

		    target.Set<Equal>(first.Operation<Multiplication>(second));

			return leftVariable;
		}
	}
}