using MilpManager.Abstraction;
using MilpManager.Utilities;
using Constraint = MilpManager.Abstraction.Constraint;

namespace MilpManager.Implementation.Constraints
{
	public class MultipleOfCalculator : IConstraintCalculator
	{
		public IVariable Set<TConstraintType>(IMilpManager milpManager, IVariable leftVariable, IVariable rightVariable) where TConstraintType : Constraint
		{
			IVariable any = milpManager.CreateAnonymous(Domain.AnyInteger);
			leftVariable.Set<Equal>(any.Operation<Multiplication>(rightVariable));

			return leftVariable;
		}
	}
}
