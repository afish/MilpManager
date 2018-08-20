namespace MilpManager.Abstraction
{
	public abstract class CompositeOperation { }
	public abstract class Approximate : CompositeOperation { }
	public abstract class Approximate2D : CompositeOperation { }
	public abstract class ArrayGet : CompositeOperation { }
	public abstract class ArraySet : CompositeOperation { }
	public abstract class CountingSort : CompositeOperation { }
	public abstract class Decomposition : CompositeOperation { }
	public abstract class Loop : CompositeOperation { }
	public abstract class IsLexicographicalEqual : CompositeOperation { }
	public abstract class IsLexicographicalGreaterOrEqual : CompositeOperation { }
	public abstract class IsLexicographicalGreaterThan : CompositeOperation { }
	public abstract class IsLexicographicalLessOrEqual : CompositeOperation { }
	public abstract class IsLexicographicalLessThan : CompositeOperation { }
	public abstract class IsLexicographicalNotEqual : CompositeOperation { }
	public abstract class OneHotEncoding : CompositeOperation { }
	public abstract class NthElements : CompositeOperation { }
	public abstract class SelectionSort : CompositeOperation { }
	public abstract class UnsignedMagnitudeDecomposition : CompositeOperation { }
}