using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public interface IConstraintCalculator
    {
        IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable, params IVariable[] rightVariable);
    }
}