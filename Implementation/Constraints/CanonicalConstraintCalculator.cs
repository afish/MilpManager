using System;
using MilpManager.Abstraction;
using MilpManager.Utilities;
using Constraint = MilpManager.Abstraction.Constraint;

namespace MilpManager.Implementation.Constraints
{
	public class CanonicalConstraintCalculator : IConstraintCalculator
	{
		public IVariable Set<TConstraintType>(IMilpManager milpManager, IVariable leftVariable,
			IVariable rightVariable) where TConstraintType : Constraint
		{
			if (typeof (TConstraintType) == typeof (Equal))
			{
				if (leftVariable != rightVariable)
				{
					milpManager.SetEqual(leftVariable, rightVariable);
					leftVariable.ConstantValue = rightVariable.ConstantValue ?? leftVariable.ConstantValue;
					rightVariable.ConstantValue = leftVariable.ConstantValue ?? rightVariable.ConstantValue;

				    if (milpManager.Settings.StoreDebugConstraints)
				    {
				        leftVariable.Constraints.Add($"== {rightVariable.FullExpression()}");
				        rightVariable.Constraints.Add($"== {leftVariable.FullExpression()}");
                    }
                }
			}
			else if (typeof (TConstraintType) == typeof (LessOrEqual))
			{
				if (leftVariable != rightVariable)
				{
					milpManager.SetLessOrEqual(leftVariable, rightVariable);

				    if (milpManager.Settings.StoreDebugConstraints)
				    {
                        leftVariable.Constraints.Add($"<= {rightVariable.FullExpression()}");
				    }
				}
			}
			else if (typeof (TConstraintType) == typeof (GreaterOrEqual))
			{
				if (leftVariable != rightVariable)
				{
					milpManager.SetGreaterOrEqual(leftVariable, rightVariable);

				    if (milpManager.Settings.StoreDebugConstraints)
				    {
				        leftVariable.Constraints.Add($">= {rightVariable.FullExpression()}");
				    }
                }
			}
			else if (typeof (TConstraintType) == typeof (LessThan))
			{
				if (leftVariable != rightVariable)
				{
					milpManager.Operation<IsLessThan>(leftVariable, rightVariable)
						.Set<Equal>(milpManager.FromConstant(1));
				}
				else
				{
					ReportInfeasibleConstraint(leftVariable, rightVariable, "less than");
				}
			}
			else if (typeof (TConstraintType) == typeof (GreaterThan))
			{
				if (leftVariable != rightVariable)
				{
					milpManager.Operation<IsGreaterThan>(leftVariable, rightVariable)
					.Set<Equal>(milpManager.FromConstant(1));
				}
				else
				{
					ReportInfeasibleConstraint(leftVariable, rightVariable, "greater than");
				}
			}
			else if (typeof (TConstraintType) == typeof (NotEqual))
				{
					if (leftVariable != rightVariable)
					{
						milpManager.Operation<IsNotEqual>(leftVariable, rightVariable)
					.Set<Equal>(milpManager.FromConstant(1));
				}
				else
				{
					ReportInfeasibleConstraint(leftVariable, rightVariable, "not equal to");
				}
			}
			else
			{
				throw new InvalidOperationException("Cannot set constraint");
			}

			return leftVariable;
		}

		private void ReportInfeasibleConstraint(IVariable leftVariable, IVariable rightVariable, string constraint)
		{
			throw new InvalidOperationException($"Setting variable to be {constraint} itself should result in infeasible model. Left variable: {leftVariable.FullExpression()}, right variable: {rightVariable.FullExpression()}");
		}
	}
}