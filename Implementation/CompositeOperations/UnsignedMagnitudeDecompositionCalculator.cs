using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class UnsignedMagnitudeDecompositionCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.UnsignedMagnitudeDecomposition && arguments.Length == 1 && (arguments[0].IsPositiveOrZero() || arguments[0].IsBinary());
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            List<Tuple<IVariable, int>> variables =
                Enumerable.Range(0, milpManager.IntegerWidth)
                    .Select(i => Tuple.Create(milpManager.CreateAnonymous(Domain.BinaryInteger), (int)Math.Pow(2, i)))
                    .ToList();

            milpManager.Operation(OperationType.Addition,
                variables.Select(v => v.Item1.Operation(OperationType.Multiplication, milpManager.FromConstant(v.Item2)))
                    .ToArray()).Set(ConstraintType.Equal, arguments[0]);

            return variables.Select(v => v.Item1);
        }
    }
}