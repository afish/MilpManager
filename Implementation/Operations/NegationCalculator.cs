using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class NegationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Negation && arguments.Length == 1;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            Domain domain = arguments[0].IsConstant()
                ? (arguments[0].IsReal() ? Domain.AnyConstantReal : Domain.AnyConstantInteger)
                : (arguments[0].IsReal() ? Domain.AnyReal : Domain.AnyInteger);


            return milpManager.NegateVariable(arguments[0], domain);
        }
    }
}
