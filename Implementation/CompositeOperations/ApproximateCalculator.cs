using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ApproximateCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return type == CompositeOperationType.Approximate && arguments.Length == 1 && parameters is ApproximateParameters;
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            var typedParameters = parameters as ApproximateParameters;
            var x = arguments.First();

            if (arguments.All(i => i.IsConstant()))
            {
                return new [] { milpManager.FromConstant(typedParameters.Function(x.ConstantValue.Value))};
            }

            var zero = milpManager.FromConstant(0);
            var one = milpManager.FromConstant(1);
            var points = typedParameters.Arguments.Select(a => Tuple.Create(milpManager.FromConstant(a), milpManager.FromConstant(typedParameters.Function(a)))).ToArray();
            var variables = points.Select(p => milpManager.CreateAnonymous(Domain.AnyReal).Set(ConstraintType.GreaterOrEqual, zero).Set(ConstraintType.LessOrEqual, one)).ToArray();

            x.Set(ConstraintType.Equal, milpManager.Operation(OperationType.Addition, points.Select((point, index) => variables[index].Operation(OperationType.Multiplication, point.Item1)).ToArray()));
            var y = milpManager.Operation(OperationType.Addition, points.Select((point, index) => variables[index].Operation(OperationType.Multiplication, point.Item2)).ToArray());

            milpManager.Operation(OperationType.Addition, variables).Set(ConstraintType.Equal, one);
            milpManager.Set(CompositeConstraintType.SpecialOrderedSetType2, variables.First(), variables.Skip(1).ToArray());

            y.ConstantValue = x.IsConstant() ? typedParameters.Function(x.ConstantValue.Value) : (double?) null;
            y.Expression = $"approximation({typedParameters.FunctionDescription})";

            return new[] {y};
        }
    }
}