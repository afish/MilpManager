using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class LoopCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return type == CompositeOperationType.Loop && (parameters as LoopParameters)?.Body?.Length == arguments.Length;
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            var options = parameters as LoopParameters;

            var totalBound = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            totalBound.Set(ConstraintType.LessOrEqual, milpManager.FromConstant(options.MaxIterations));

            for (int i = 1; i <= options.MaxIterations; ++i)
            {
                var counter = milpManager.FromConstant(i);
                var isLooping = counter.Operation(OperationType.IsLessOrEqual, totalBound);
                for (int v = 0; v < arguments.Length; ++v)
                {
                    arguments[v] = milpManager.Operation(OperationType.Condition, isLooping, options.Body[v](counter, arguments), arguments[v]);
                }
            }

            return arguments.Concat(new[] {totalBound}).ToArray();
        }
    }
}