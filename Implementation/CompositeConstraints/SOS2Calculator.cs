using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class SOS2Calculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            var maximumIntegerValue = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            var one = milpManager.FromConstant(1);

            var allVariables = new[] { leftVariable }.Concat(rightVariable).ToArray();
            var boundaryVariables = allVariables.Select(v => milpManager.CreateAnonymous(Domain.BinaryInteger)).ToArray();
            milpManager.Operation(OperationType.Addition, boundaryVariables).Set(ConstraintType.LessOrEqual, one);

            for (int i = 0; i < allVariables.Length; ++i)
            {
                IVariable sum = boundaryVariables[i];
                if (i < allVariables.Length - 1)
                {
                    sum = sum.Operation(OperationType.Addition, boundaryVariables[i + 1]);
                }
                allVariables[i]
                    .Set(ConstraintType.LessOrEqual, sum.Operation(OperationType.Multiplication, maximumIntegerValue))
                    .Set(ConstraintType.GreaterOrEqual, sum.Operation(OperationType.Multiplication, maximumIntegerValue).Operation(OperationType.Negation));
            }

            return leftVariable;
        }
    }
}