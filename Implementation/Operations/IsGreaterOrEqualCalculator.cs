using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsGreaterOrEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsGreaterOrEqual && arguments.Length == 2 && arguments.All(x => x.IsInteger());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            return arguments[0].Operation(OperationType.IsLessThan, arguments[1])
                .Operation(OperationType.BinaryNegation);
        }
    }
}
