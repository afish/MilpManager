using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ConditionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Condition &&  arguments.Length == 3 && arguments[0].IsBinary() && arguments[1].IsInteger() && arguments[2].IsInteger();
        }

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            return baseMilpManager.Create(baseMilpManager.Operation(OperationType.Addition,
                arguments[0].Operation(OperationType.Multiplication, arguments[1]),
                arguments[0].Operation(OperationType.BinaryNegation)
                    .Operation(OperationType.Multiplication, arguments[2])
                ));
        }
    }
}
