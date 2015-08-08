using MilpManager.Abstraction;

namespace MilpManager.Implementation.Comparisons
{
    public interface IConstraintCalculator
    {
        IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable, IVariable rightVariable);
    }
}