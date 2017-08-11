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
            var maximumIntegerValue = milpManager.FromConstant(milpManager.MaximumIntegerValue);

            var allVariables = new[] {leftVariable}.Concat(rightVariable).ToArray();
            var boundaryVariables = allVariables.Select(v => milpManager.CreateAnonymous(Domain.BinaryInteger)).ToArray();
            milpManager.Operation<Addition>(boundaryVariables).Set<LessOrEqual>(one);
            foreach (var pair in allVariables.Zip(boundaryVariables, Tuple.Create))
            {
                pair.Item1
                    .Set<LessOrEqual>(pair.Item2.Operation<Multiplication>(maximumIntegerValue))
                    .Set<GreaterOrEqual>(pair.Item2.Operation<Multiplication>(maximumIntegerValue).Operation<Negation>());
            }

            return leftVariable;
        }
    }
}