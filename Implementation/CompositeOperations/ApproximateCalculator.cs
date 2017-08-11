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
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
            var typedParameters = parameters as ApproximateParameters;
            var x = arguments.First();

            if (arguments.All(i => i.IsConstant()))
            {
                return new [] { milpManager.FromConstant(typedParameters.Function(x.ConstantValue.Value))};
            }
            
            var one = milpManager.FromConstant(1);
            var points = typedParameters.Arguments.Select(a => Tuple.Create(milpManager.FromConstant(a), milpManager.FromConstant(typedParameters.Function(a)))).ToArray();
            var variables = points.Select(p => milpManager.CreateAnonymous(typedParameters.ArgumentMustBeOnAGrid ? Domain.BinaryInteger : Domain.PositiveOrZeroReal).Set<LessOrEqual>(one)).ToArray();

            x.Set<Equal>(milpManager.Operation<Addition>(points.Select((point, index) => variables[index].Operation<Multiplication>(point.Item1)).ToArray()));
            var y = milpManager.Operation<Addition>(points.Select((point, index) => variables[index].Operation<Multiplication>(point.Item2)).ToArray());

            milpManager.Operation<Addition>(variables).Set<Equal>(one);
            milpManager.Set(CompositeConstraintType.SpecialOrderedSetType2, variables.First(), variables.Skip(1).ToArray());

            y.ConstantValue = x.IsConstant() ? typedParameters.Function(x.ConstantValue.Value) : (double?) null;
            y.Expression = $"approximation({typedParameters.FunctionDescription})";

            return new[] {y};
        }
    }
}