using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Implementation;
using MilpManager.Implementation.CompositeConstraints;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Constraints;
using MilpManager.Implementation.Operations;

namespace MilpManager.Abstraction
{
    public abstract class BaseMilpManager : IMilpManager
    {
        protected readonly IDictionary<ConstraintType, IConstraintCalculator> Constraints = new Dictionary
            <ConstraintType, IConstraintCalculator>
        {
            {ConstraintType.Equal, new CanonicalConstraintCalculator()},
            {ConstraintType.LessOrEqual, new CanonicalConstraintCalculator()},
            {ConstraintType.GreaterOrEqual, new CanonicalConstraintCalculator()},
            {ConstraintType.LessThan, new CanonicalConstraintCalculator()},
            {ConstraintType.GreaterThan, new CanonicalConstraintCalculator()},
            {ConstraintType.NotEqual, new CanonicalConstraintCalculator()},
            {ConstraintType.MultipleOf, new MultipleOfCalculator()}
        };

        protected readonly IDictionary<CompositeConstraintType, ICompositeConstraintCalculator> CompositeConstraints = new Dictionary
            <CompositeConstraintType, ICompositeConstraintCalculator>
        {
            {CompositeConstraintType.FromSet, new FromSetCalculator()},
            {CompositeConstraintType.NotFromSet, new NotFromSetCalculator()},
            {CompositeConstraintType.AllDifferent, new AllDifferentCalculator()},
            {CompositeConstraintType.NDifferent, new NDifferentCalculator()}
        };

        protected readonly IDictionary<CompositeOperationType, ICompositeOperationCalculator> CompositeOperations = new Dictionary
            <CompositeOperationType, ICompositeOperationCalculator>
        {
            {CompositeOperationType.UnsignedMagnitudeDecomposition, new UnsignedMagnitudeDecompositionCalculator()},
            {CompositeOperationType.SelectionSort, new SelectionSortCalculator()},
            {CompositeOperationType.NthElements, new NthElementsCalculator()},
            {CompositeOperationType.Loop, new LoopCalculator()},
            {CompositeOperationType.CountingSort, new CountingSortCalculator()}
        };

        protected readonly IDictionary<OperationType, IOperationCalculator> Operations = new Dictionary
            <OperationType, IOperationCalculator>
        {
            {OperationType.Addition, new AdditionCalculator()},
            {OperationType.Subtraction, new SubtractionCalculator()},
            {OperationType.Multiplication, new MultiplicationCalculator()},
            {OperationType.Division, new DivisionCalculator()},
            {OperationType.Conjunction, new ConjunctionCalculator()},
            {OperationType.Disjunction, new DisjunctionCalculator()},
            {OperationType.IsGreaterThan, new IsGreaterThanCalculator()},
            {OperationType.IsLessThan, new IsLessThanCalculator()},
            {OperationType.BinaryNegation, new BinaryNegationCalculator()},
            {OperationType.IsEqual, new IsEqualCalculator()},
            {OperationType.IsNotEqual, new IsNotEqualCalculator()},
            {OperationType.Negation, new NegationCalculator()},
            {OperationType.AbsoluteValue, new AbsoluteValueCalculator()},
            {OperationType.Maximum, new MaximumMinimumCalculator()},
            {OperationType.Minimum, new MaximumMinimumCalculator()},
            {OperationType.Condition, new ConditionCalculator()},
            {OperationType.IsGreaterOrEqual, new IsGreaterOrEqualCalculator()},
            {OperationType.IsLessOrEqual, new IsLessOrEqualCalculator()},
            {OperationType.Remainder, new RemainderCalculator()},
            {OperationType.GCD, new GcdCalculator()},
            {OperationType.Exponentation, new ExponentationCalculator()},
            {OperationType.Factorial, new FactorialCalculator()},
            {OperationType.DifferentValuesCount, new DifferentValuesCountCalculator()}
        };

        protected BaseMilpManager(int integerWidth)
        {
            IntegerWidth = integerWidth;
        }
        private static Domain SelectDomainForConstant(IVariable value)
        {
            return value.Domain == Domain.AnyConstantInteger
                ? Domain.AnyInteger
                : value.Domain == Domain.AnyConstantReal
                    ? Domain.AnyReal
                    : value.Domain == Domain.BinaryConstantInteger
                        ? Domain.BinaryInteger
                        : value.Domain == Domain.PositiveOrZeroConstantInteger
                            ? Domain.PositiveOrZeroInteger
                            : value.Domain == Domain.PositiveOrZeroConstantReal
                                ? Domain.PositiveOrZeroReal
                                : value.Domain;
        }

        public virtual int IntegerWidth { get; private set; }

        public virtual int IntegerInfinity
        {
            get { return (int) Math.Pow(2, IntegerWidth + 4) + 1; }
        }

        public virtual int MaximumIntegerValue
        {
            get { return (int) Math.Pow(2, IntegerWidth) - 1; }
        }

        public virtual IVariable Create(string name, IVariable value)
        {
            var variable = Create(name,
                SelectDomainForConstant(value));
            Set(ConstraintType.Equal, variable, value);
            return variable;
        }

        public virtual IVariable Create(IVariable value)
        {
            var variable = CreateAnonymous(SelectDomainForConstant(value));
            Set(ConstraintType.Equal, variable, value);
            return variable;
        }

        public virtual IVariable Operation(OperationType type, params IVariable[] variables)
        {
            if (Operations[type].SupportsOperation(type, variables))
            {
                return Operations[type].Calculate(this, type, variables);
            }

            throw new NotSupportedException("Operation " + type + " with supplied variables [" +
                                            string.Join(", ", variables.Select(v => v.Domain.ToString()).ToArray()) +
                                            "] not supported");
        }

        public virtual IVariable Set(ConstraintType type, IVariable left, IVariable right)
        {
            return Constraints[type].Set(this, type, left, right);
        }

        public virtual IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            params IVariable[] variables)
        {
            return CompositeOperation(type, null, variables);
        }

        public virtual IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            ICompositeOperationParameters parameters, params IVariable[] variables)
        {
            if (CompositeOperations[type].SupportsOperation(type, parameters, variables))
            {
                return CompositeOperations[type].Calculate(this, type, parameters, variables);
            }

            throw new NotSupportedException(
                $"Operation {type} with supplied variables [{string.Join(", ", variables.Select(v => v.Domain.ToString()).ToArray())}] with parameters {parameters} not supported");
        }

        public virtual IVariable Set(CompositeConstraintType type, IVariable left, params IVariable[] variables)
        {
            return Set(type, null, left, variables);
        }

        public virtual IVariable Set(CompositeConstraintType type, ICompositeConstraintParameters parameters, IVariable left,
            params IVariable[] variables)
        {
            return CompositeConstraints[type].Set(this, type, parameters, left, variables);
        }

        public virtual void SetGreaterOrEqual(IVariable variable, IVariable bound)
        {
            bound.Set(ConstraintType.LessOrEqual, variable);
        }

        public virtual void SetEqual(IVariable variable, IVariable bound)
        {
            bound.Set(ConstraintType.LessOrEqual, variable);
            variable.Set(ConstraintType.LessOrEqual, bound);
        }

        public virtual IVariable FromConstant(double value)
        {
            return FromConstant(value, value < 0 ? Domain.AnyConstantReal : Domain.PositiveOrZeroConstantReal);
        }

        public virtual IVariable FromConstant(int value)
        {
            var domain = value < 0
                ? Domain.AnyConstantInteger
                : value > 1 ? Domain.PositiveOrZeroConstantInteger : Domain.BinaryConstantInteger;
            return FromConstant(value, domain);
        }

        public abstract IVariable SumVariables(IVariable first, IVariable second, Domain domain);
        public abstract IVariable NegateVariable(IVariable variable, Domain domain);
        public abstract IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract void SetLessOrEqual(IVariable variable, IVariable bound);
        public abstract IVariable FromConstant(int value, Domain domain);
        public abstract IVariable FromConstant(double value, Domain domain);
        public abstract IVariable Create(string name, Domain domain);
        public abstract IVariable CreateAnonymous(Domain domain);
    }
}