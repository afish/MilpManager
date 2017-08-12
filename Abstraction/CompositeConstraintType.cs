namespace MilpManager.Abstraction
{
	public abstract class CompositeConstraintType { }
	public abstract class AllDifferent : CompositeConstraintType { }
	public abstract class Cardinality : CompositeConstraintType { }
	public abstract class FromSet : CompositeConstraintType { }
	public abstract class NotFromSet : CompositeConstraintType { }
	public abstract class SpecialOrderedSetType1 : CompositeConstraintType { }
	public abstract class SpecialOrderedSetType2 : CompositeConstraintType { }
}