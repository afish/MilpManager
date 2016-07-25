using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class SOS1Calculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            var one = milpManager.FromConstant(1);
            var allVariables = new[] {leftVariable}.Concat(rightVariable).ToArray();
            var boundaryVariables = allVariables.Select(v => milpManager.CreateAnonymous(Domain.BinaryInteger)).ToArray();
            milpManager.Operation(OperationType.Addition, boundaryVariables).Set(ConstraintType.LessOrEqual, one);
            foreach (var pair in allVariables.Zip(boundaryVariables, Tuple.Create))
            {
                pair.Item1.Set(ConstraintType.LessOrEqual, pair.Item2);
            }

            return leftVariable;
        }
    }
}