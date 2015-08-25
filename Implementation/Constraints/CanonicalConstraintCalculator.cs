using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    internal class CanonicalConstraintCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable,
            params IVariable[] rightVariable)
        {
            if (type == ConstraintType.Equal)
            {
                milpManager.SetEqual(leftVariable, rightVariable[0]);
            }
            else if (type == ConstraintType.GreaterOrEqual)
            {
                milpManager.SetGreaterOrEqual(leftVariable, rightVariable[0]);
            }
            else if (type == ConstraintType.LessOrEqual)
            {
                milpManager.SetLessOrEqual(leftVariable, rightVariable[0]);
            }
            return leftVariable;
        }
    }
}