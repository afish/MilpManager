using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    internal class CanonicalConstraintCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable,
            IVariable rightVariable)
        {
            if (type == ConstraintType.Equal)
            {
                milpManager.SetEqual(leftVariable, rightVariable);
            }
            else if (type == ConstraintType.GreaterOrEqual)
            {
                milpManager.SetGreaterOrEqual(leftVariable, rightVariable);
            }
            else if (type == ConstraintType.LessOrEqual)
            {
                milpManager.SetLessOrEqual(leftVariable, rightVariable);
            }
            return leftVariable;
        }
    }
}