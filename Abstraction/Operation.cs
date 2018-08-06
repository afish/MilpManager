namespace MilpManager.Abstraction
{
	public abstract class Operation { }
	public abstract class AbsoluteValue : Operation { }
	public abstract class Addition : Operation { }
	public abstract class BinaryNegation : Operation { }
	public abstract class Condition : Operation { }
	public abstract class Conjunction : Operation { }
	public abstract class DifferentValuesCount : Operation { }
	public abstract class Disjunction : Operation { }
    public abstract class Division : Operation { }
    public abstract class Equivalency : Operation { }
	public abstract class ExclusiveDisjunction : Operation { }
	public abstract class Exponentiation : Operation { }
	public abstract class GCD : Operation { }
	public abstract class Factorial : Operation { }
    public abstract class IsEqual : Operation { }
	public abstract class IsGreaterOrEqual : Operation { }
	public abstract class IsGreaterThan : Operation { }
	public abstract class IsLessOrEqual : Operation { }
	public abstract class IsLessThan : Operation { }
	public abstract class IsNotEqual : Operation { }
	public abstract class MaterialImplication : Operation { }
	public abstract class Maximum : Operation { }
	public abstract class Minimum : Operation { }
	public abstract class Multiplication : Operation { }
	public abstract class Negation : Operation { }
	public abstract class Subtraction : Operation { }
	public abstract class RealDivision : Operation { }
	public abstract class Remainder : Operation { }
}