using System;
using System.Collections.Generic;
using MilpManager.Implementation.CompositeConstraints;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Constraints;
using MilpManager.Implementation.Goals;
using MilpManager.Implementation.Operations;

namespace MilpManager.Abstraction
{
	public static class DefaultCalculators
	{
		public static IDictionary<Type, IConstraintCalculator> Constraints => new Dictionary
		   <Type, IConstraintCalculator>
		{
			{typeof(Equal), new CanonicalConstraintCalculator()},
			{typeof(GreaterOrEqual), new CanonicalConstraintCalculator()},
			{typeof(GreaterThan), new CanonicalConstraintCalculator()},
			{typeof(LessOrEqual), new CanonicalConstraintCalculator()},
			{typeof(LessThan), new CanonicalConstraintCalculator()},
			{typeof(NotEqual), new CanonicalConstraintCalculator()},
			{typeof(MultipleOf), new MultipleOfCalculator()}
		};
		public static IDictionary<Type, IOperationCalculator> Operations => new Dictionary
			<Type, IOperationCalculator>
		{
			{typeof(AbsoluteValue), new AbsoluteValueCalculator()},
			{typeof(Addition), new AdditionCalculator()},
			{typeof(BinaryNegation), new BinaryNegationCalculator()},
			{typeof(Condition), new ConditionCalculator()},
			{typeof(Conjunction), new ConjunctionCalculator()},
			{typeof(DifferentValuesCount), new DifferentValuesCountCalculator()},
			{typeof(Disjunction), new DisjunctionCalculator()},
			{typeof(Equivalency), new EquivalencyCalculator()},
			{typeof(ExclusiveDisjunction), new ExclusiveDisjunctionCalculator()},
			{typeof(Exponentiation), new ExponentiationCalculator()},
			{typeof(Factorial), new FactorialCalculator()},
			{typeof(GCD), new GcdCalculator()},
		    {typeof(Division), new DivisionCalculator()},
            {typeof(Subtraction), new SubtractionCalculator()},
			{typeof(MaterialImplication), new MaterialImplicationCalculator()},
			{typeof(Multiplication), new MultiplicationCalculator()},
			{typeof(IsEqual), new IsEqualCalculator()},
			{typeof(IsGreaterOrEqual), new IsGreaterOrEqualCalculator()},
			{typeof(IsGreaterThan), new IsGreaterThanCalculator()},
			{typeof(IsLessOrEqual), new IsLessOrEqualCalculator()},
			{typeof(IsLessThan), new IsLessThanCalculator()},
			{typeof(IsNotEqual), new IsNotEqualCalculator()},
			{typeof(Maximum), new MaximumMinimumCalculator()},
			{typeof(Minimum), new MaximumMinimumCalculator()},
			{typeof(Negation), new NegationCalculator()},
			{typeof(RealDivision), new RealDivisionCalculator()},
			{typeof(Remainder), new RemainderCalculator()},
		    {typeof(Truncation), new TruncationCalculator()}
        };

		public static IDictionary<Type, ICompositeOperationCalculator> CompositeOperations => new Dictionary
			<Type, ICompositeOperationCalculator>
		{
			{typeof(Approximate), new ApproximateCalculator()},
			{typeof(Approximate2D), new Approximate2DCalculator()},
			{typeof(ArrayGet), new ArrayGetCalculator()},
			{typeof(ArraySet), new ArraySetCalculator()},
			{typeof(CountingSort), new CountingSortCalculator()},
			{typeof(Decomposition), new DecompositionCalculator()},
			{typeof(IsLexicographicalEqual), new IsLexicographicalEqualCalculator()},
			{typeof(IsLexicographicalGreaterOrEqual), new IsLexicographicalGreaterOrEqualCalculator()},
			{typeof(IsLexicographicalGreaterThan), new IsLexicographicalGreaterThanCalculator()},
			{typeof(IsLexicographicalLessOrEqual), new IsLexicographicalLessOrEqualCalculator()},
			{typeof(IsLexicographicalLessThan), new IsLexicographicalLessThanCalculator()},
			{typeof(IsLexicographicalNotEqual), new IsLexicographicalNotEqualCalculator()},
			{typeof(Loop), new LoopCalculator()},
			{typeof(NthElements), new NthElementsCalculator()},
			{typeof(SelectionSort), new SelectionSortCalculator()},
			{typeof(UnsignedMagnitudeDecomposition), new UnsignedMagnitudeDecompositionCalculator()}
		};

		public static IDictionary<Type, ICompositeConstraintCalculator> CompositeConstraints => new Dictionary
			<Type, ICompositeConstraintCalculator>
		{
			{typeof(AllDifferent), new AllDifferentCalculator()},
			{typeof(Cardinality), new CardinalityCalculator()},
			{typeof(FromSet), new FromSetCalculator()},
			{typeof(NotFromSet), new NotFromSetCalculator()},
			{typeof(SpecialOrderedSetType1), new SOS1Calculator()},
			{typeof(SpecialOrderedSetType2), new SOS2Calculator()}
		};

		public static IDictionary<Type, IGoalCalculator> GoalCalculators => new Dictionary<Type, IGoalCalculator>
		{
			{typeof(Minimize), new MinimizeCalculator() },
			{typeof(Maximize), new MaximizeCalculator() },
			{typeof(MaximizeMinimum), new MaximizeMinimumCalculator() },
			{typeof(MinimizeMaximum), new MinimizeMaximumCalculator() },
			{typeof(MaximizeMaximum), new MaximizeMaximumCalculator() },
			{typeof(MinimizeMinimum), new MinimizeMinimumCalculator() }
		};
	}
}