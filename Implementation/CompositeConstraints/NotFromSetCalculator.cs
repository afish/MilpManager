using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class NotFromSetCalculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        { 
            rightVariable.Aggregate(milpManager.FromConstant(0),
                (current, variable) =>
                    current.Operation(OperationType.Addition, leftVariable.Operation(OperationType.IsEqual, variable))).Create()
                .Set(ConstraintType.Equal, milpManager.FromConstant(0));

            return leftVariable;
        }
    }
}