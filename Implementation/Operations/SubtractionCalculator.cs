using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class SubtractionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Subtraction && arguments.Length == 2;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            return
                arguments.Aggregate(
                    (left, right) => left.Operation(OperationType.Addition, right.Operation(OperationType.Negation)));
        }
    }
}
