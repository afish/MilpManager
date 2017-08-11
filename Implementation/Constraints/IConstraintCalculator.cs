using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public interface IConstraintCalculator
    {
        IVariable Set<TConstraintType>(IMilpManager milpManager, IVariable leftVariable, IVariable rightVariable) where TConstraintType : ConstraintType;
    }
}