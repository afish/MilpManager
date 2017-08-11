using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
	public class CanonicalConstraintCalculator : IConstraintCalculator
	{
		public IVariable Set<TConstraintType>(IMilpManager milpManager, IVariable leftVariable,
			IVariable rightVariable) where TConstraintType : ConstraintType
		{
			if (typeof (TConstraintType) == typeof (Equal))
			{
				milpManager.SetEqual(leftVariable, rightVariable);
				leftVariable.ConstantValue = rightVariable.ConstantValue ?? leftVariable.ConstantValue;
				rightVariable.ConstantValue = leftVariable.ConstantValue ?? rightVariable.ConstantValue;
			}
			else if (typeof (TConstraintType) == typeof (LessOrEqual))
			{
				milpManager.SetLessOrEqual(leftVariable, rightVariable);
			}
			else if (typeof (TConstraintType) == typeof (GreaterOrEqual))
			{
				milpManager.SetGreaterOrEqual(leftVariable, rightVariable);
			}
			else if (typeof (TConstraintType) == typeof (LessThan))
			{
				milpManager.Operation<IsLessThan>(leftVariable, rightVariable)
					.Set<Equal>(milpManager.FromConstant(1));
			}
			else if (typeof (TConstraintType) == typeof (GreaterThan))
			{
				milpManager.Operation<IsGreaterThan>(leftVariable, rightVariable)
					.Set<Equal>(milpManager.FromConstant(1));
			}
			else if (typeof (TConstraintType) == typeof (NotEqual))
			{
				milpManager.Operation<IsNotEqual>(leftVariable, rightVariable)
					.Set<Equal>(milpManager.FromConstant(1));
			}
			else
			{
				throw new InvalidOperationException("Cannot set constraint");
			}

			return leftVariable;
		}
	}
}