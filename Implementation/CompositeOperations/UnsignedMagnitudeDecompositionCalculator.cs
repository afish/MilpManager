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
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
            var decomposition = milpManager.CompositeOperation(CompositeOperationType.Decomposition, new DecompositionParameters { Base = 2 }, arguments);
            if (arguments[0].IsConstant())
            {
                return decomposition;
            }

            return decomposition.Zip(Enumerable.Range(0, milpManager.IntegerWidth), (v, index) =>
            {
				SolverUtilities.SetExpression(v, $"unsignedMagnitudeDecomposition(bit: {index}, {arguments[0].FullExpression()})");
                return v;
            });
        }
    }
}