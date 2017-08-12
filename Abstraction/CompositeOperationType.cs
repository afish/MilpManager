namespace MilpManager.Abstraction
{
	public abstract class CompositeOperationType { }
	public abstract class Approximate : CompositeOperationType { }
	public abstract class Approximate2D : CompositeOperationType { }
	public abstract class ArrayGet : CompositeOperationType { }
	public abstract class ArraySet : CompositeOperationType { }
	public abstract class CountingSort : CompositeOperationType { }
	public abstract class Decomposition : CompositeOperationType { }
	public abstract class Loop : CompositeOperationType { }
	public abstract class IsLexicographicalEqual : CompositeOperationType { }
	public abstract class IsLexicographicalGreaterOrEqual : CompositeOperationType { }
	public abstract class IsLexicographicalGreaterThan : CompositeOperationType { }
	public abstract class IsLexicographicalLessOrEqual : CompositeOperationType { }
	public abstract class IsLexicographicalLessThan : CompositeOperationType { }
	public abstract class IsLexicographicalNotEqual : CompositeOperationType { }
	public abstract class NthElements : CompositeOperationType { }
	public abstract class SelectionSort : CompositeOperationType { }
	public abstract class UnsignedMagnitudeDecomposition : CompositeOperationType { }
}