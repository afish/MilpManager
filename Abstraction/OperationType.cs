namespace MilpManager.Abstraction
{
	public abstract class OperationType { }
	public abstract class AbsoluteValue : OperationType { }
	public abstract class Addition : OperationType { }
	public abstract class BinaryNegation : OperationType { }
	public abstract class Condition : OperationType { }
	public abstract class Conjunction : OperationType { }
	public abstract class DifferentValuesCount : OperationType { }
	public abstract class Disjunction : OperationType { }
    public abstract class Division : OperationType { }
    public abstract class Equivalency : OperationType { }
	public abstract class ExclusiveDisjunction : OperationType { }
	public abstract class Exponentiation : OperationType { }
	public abstract class GCD : OperationType { }
	public abstract class Factorial : OperationType { }
    public abstract class IsEqual : OperationType { }
	public abstract class IsGreaterOrEqual : OperationType { }
	public abstract class IsGreaterThan : OperationType { }
	public abstract class IsLessOrEqual : OperationType { }
	public abstract class IsLessThan : OperationType { }
	public abstract class IsNotEqual : OperationType { }
	public abstract class MaterialImplication : OperationType { }
	public abstract class Maximum : OperationType { }
	public abstract class Minimum : OperationType { }
	public abstract class Multiplication : OperationType { }
	public abstract class Negation : OperationType { }
	public abstract class Subtraction : OperationType { }
	public abstract class Remainder : OperationType { }
}