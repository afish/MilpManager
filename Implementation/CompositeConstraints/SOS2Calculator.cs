using System;
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

            var nonZeroes = new [] {leftVariable}.Concat(rightVariable).Select(v => v.Operation(OperationType.IsNotEqual, zero)).ToArray();
            var nonZeroPairs = nonZeroes.Zip(nonZeroes.Skip(1), Tuple.Create).Select(pair => pair.Item1.Operation(OperationType.Conjunction, pair.Item2)).ToArray();
            var nonZeroesCount = milpManager.Operation(OperationType.Addition, nonZeroes);
            milpManager.Set(
                ConstraintType.Equal,
                milpManager.Operation(
                    OperationType.Disjunction,
                    nonZeroesCount.Operation(OperationType.IsEqual, zero),
                    nonZeroesCount.Operation(OperationType.IsEqual, one),
                    milpManager.Operation(OperationType.Conjunction,
                        nonZeroesCount.Operation(OperationType.IsEqual, milpManager.FromConstant(2)), 
                        milpManager.Operation(OperationType.Addition, nonZeroPairs).Operation(OperationType.IsEqual, one)
                    )
                ),
                one
            );

            return leftVariable;
        }
    }
}