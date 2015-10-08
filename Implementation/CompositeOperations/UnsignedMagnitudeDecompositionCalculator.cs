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
            return type == CompositeOperationType.UnsignedMagnitudeDecomposition && arguments.Length == 1 &&
                   (arguments[0].IsPositiveOrZero() || arguments[0].IsBinary()) && arguments[0].IsInteger();
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            return InternalCalculate(milpManager, parameters, arguments);
        }

        private IEnumerable<IVariable> InternalCalculate(IMilpManager milpManager, ICompositeOperationParameters parameters, IVariable[] arguments)
        {
            if (arguments[0].IsConstant())
            {
                int currentValue = (int)arguments[0].ConstantValue.Value;
                for (int i = 0; i < milpManager.IntegerWidth; ++i)
                {
                    yield return milpManager.FromConstant(currentValue % 2);
                    currentValue /= 2;
                }
                yield break;
            }

            foreach (var i in CalculateForVariable(milpManager, parameters, arguments))
            {
                yield return i;
            }
        }

        private IEnumerable<IVariable> CalculateForVariable(IMilpManager milpManager, ICompositeOperationParameters parameters, IVariable[] arguments)
        {
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