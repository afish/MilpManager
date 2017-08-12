using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public interface ICompositeConstraintCalculator
    {
        IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters, IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraintType;
    }
}