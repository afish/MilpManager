using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class DisjunctionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Disjunction && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            var variable = milpManager.CreateAnonymous(arguments.Any(a => a.IsNotConstant()) ? Domain.BinaryInteger : Domain.BinaryConstantInteger);
            var sum = milpManager.Operation(OperationType.Addition, arguments);
            var argumentsCount = arguments.Length;
            sum.Operation(OperationType.Subtraction,
                milpManager.FromConstant(argumentsCount).Operation(OperationType.Multiplication, variable))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(0))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(-(argumentsCount - 1)));

            return variable;
        }
    }
}
