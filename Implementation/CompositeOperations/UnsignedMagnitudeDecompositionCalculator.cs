using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    internal class UnsignedMagnitudeDecompositionCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.UnsignedMagnitudeDecomposition && arguments.Length == 1 && (arguments[0].IsPositiveOrZero() || arguments[0].IsBinary());
        }

        public IEnumerable<IVariable> Calculate(BaseMilpManager baseMilpManager, CompositeOperationType type,
            ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            List<Tuple<IVariable, int>> variables =
                Enumerable.Range(0, baseMilpManager.IntegerWidth)
                    .Select(i => Tuple.Create(baseMilpManager.CreateAnonymous(Domain.BinaryInteger), (int)Math.Pow(2, i)))
                    .ToList();

            baseMilpManager.Operation(OperationType.Addition,
                variables.Select(v => v.Item1.Operation(OperationType.Multiplication, baseMilpManager.FromConstant(v.Item2)))
                    .ToArray()).Set(ConstraintType.Equal, arguments[0]);

            return variables.Select(v => v.Item1);
        }
    }
}