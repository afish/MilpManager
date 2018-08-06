using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeConstraints
{
	public class AllDifferentCalculator : ICompositeConstraintCalculator
	{
		public IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters,
			IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraintType

		{
			leftVariable.Operation<DifferentValuesCount>(rightVariable)
				.Set<Equal>(milpManager.FromConstant(rightVariable.Length + 1));

			return leftVariable;
		}
	}
}