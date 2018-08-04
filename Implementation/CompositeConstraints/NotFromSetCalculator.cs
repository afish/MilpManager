using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
	public class NotFromSetCalculator : ICompositeConstraintCalculator
	{
		public IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters,
			IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraintType

		{ 
			rightVariable.Aggregate(milpManager.FromConstant(0),
				(current, variable) =>
					current.Operation<Addition>(leftVariable.Operation<IsEqual>(variable))).Create()
				.Set<Equal>(milpManager.FromConstant(0));

			return leftVariable;
		}
	}
}