using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public class FromSetCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable, params IVariable[] rightVariable)
        {
            rightVariable.Aggregate(milpManager.FromConstant(0),
                (current, variable) =>
                    current.Operation(OperationType.Addition, leftVariable.Operation(OperationType.IsEqual, variable))).Create()
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(1));

            return leftVariable;
        }
    }
}