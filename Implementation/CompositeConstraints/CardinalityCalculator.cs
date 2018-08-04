using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
	public class CardinalityCalculator : ICompositeConstraintCalculator
	{
		public IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters,
			IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraintType

		{
			leftVariable.Operation<DifferentValuesCount>(rightVariable)
				.Set<Equal>(milpManager.FromConstant((parameters as CardinalityParameters).ValuesCount));

			return leftVariable;
		}
	}
}