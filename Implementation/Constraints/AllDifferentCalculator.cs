using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public class AllDifferentCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable, params IVariable[] rightVariable)
        {
            leftVariable.Operation(OperationType.DifferentValuesCount, rightVariable)
                .Set(ConstraintType.Equal, milpManager.FromConstant(rightVariable.Length + 1));

            return leftVariable;
        }
    }
}