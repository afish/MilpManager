using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class BinaryNegationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.BinaryNegation && arguments.Length == 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            return baseMilpManager.FromConstant(1).Operation(OperationType.Subtraction, arguments[0]).ChangeDomain(Domain.BinaryInteger).Create();
        }
    }
}
