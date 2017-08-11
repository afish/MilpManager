using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class FromSetCalculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            rightVariable.Aggregate(milpManager.FromConstant(0),
                (current, variable) =>
                    current.Operation<Addition>(leftVariable.Operation<IsEqual>(variable))).Create()
                .Set<GreaterOrEqual>(milpManager.FromConstant(1));

            return leftVariable;
        }
    }
}