using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class SOS2Calculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            var zero = milpManager.FromConstant(0);
            var one = milpManager.FromConstant(1);


            var allVariables = new[] { leftVariable }.Concat(rightVariable).ToArray();
            var boundaryVariables = allVariables.Select(v => milpManager.CreateAnonymous(Domain.BinaryInteger)).ToArray();
            milpManager.Operation(OperationType.Addition, boundaryVariables).Set(ConstraintType.LessOrEqual, one);
            for (int i = 0; i < allVariables.Length; ++i)
            {
                IVariable sum = boundaryVariables[i];
                if (i > 0)
                {
                    sum = sum.Operation(OperationType.Addition, boundaryVariables[i - 1]);
                }
                allVariables[i].Set(ConstraintType.LessOrEqual, sum);
            }

            return leftVariable;
        }
    }
}