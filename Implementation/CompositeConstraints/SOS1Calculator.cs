using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class SOS1Calculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            var zero = milpManager.FromConstant(0);
            var one = milpManager.FromConstant(1);
            var nonZeroes = new [] {leftVariable}.Concat(rightVariable).Select(v => v.Operation(OperationType.IsNotEqual, zero)).ToArray();
            var nonZeroesCount = milpManager.Operation(OperationType.Addition, nonZeroes);
            nonZeroesCount.Set(ConstraintType.LessOrEqual, one);

            return leftVariable;
        }
    }
}