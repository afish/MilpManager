using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public interface ICompositeConstraintCalculator
    {
        IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters, IVariable leftVariable, params IVariable[] rightVariable);
    }
}