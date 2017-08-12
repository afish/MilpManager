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
			{typeof(Division), new DivisionCalculator()},
			{typeof(Equivalency), new EquivalencyCalculator()},
			{typeof(ExclusiveDisjunction), new ExclusiveDisjunctionCalculator()},
			{typeof(Exponentiation), new ExponentiationCalculator()},
			{typeof(Factorial), new FactorialCalculator()},
			{typeof(GCD), new GcdCalculator()},
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
			{typeof(Remainder), new RemainderCalculator()}
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

		public static IDictionary<CompositeConstraintType, ICompositeConstraintCalculator> CompositeConstraints => new Dictionary
			<CompositeConstraintType, ICompositeConstraintCalculator>
		{
			{CompositeConstraintType.AllDifferent, new AllDifferentCalculator()},
			{CompositeConstraintType.Cardinality, new CardinalityCalculator()},
			{CompositeConstraintType.FromSet, new FromSetCalculator()},
			{CompositeConstraintType.NotFromSet, new NotFromSetCalculator()},
			{CompositeConstraintType.SpecialOrderedSetType1, new SOS1Calculator()},
			{CompositeConstraintType.SpecialOrderedSetType2, new SOS2Calculator()}
		};

		public static IDictionary<GoalType, IGoalCalculator> GoalCalculators => new Dictionary<GoalType, IGoalCalculator>
		{
			{GoalType.Minimize, new MinimizeCalculator() },
			{GoalType.Maximize, new MaximizeCalculator() },
			{GoalType.MaximizeMinium, new MaximizeMinimumCalculator() },
			{GoalType.MinimizeMaximum, new MinimizeMaximumCalculator() },
			{GoalType.MaximizeMaximum, new MaximizeMaximumCalculator() },
			{GoalType.MinimizeMinimum, new MinimizeMinimumCalculator() }
		};
	}
}