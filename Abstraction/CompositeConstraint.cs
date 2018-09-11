namespace MilpManager.Abstraction
{
	public abstract class CompositeConstraint { }
	public abstract class AllDifferent : CompositeConstraint { }
	public abstract class Cardinality : CompositeConstraint { }
	public abstract class Composite : CompositeConstraint { }
	public abstract class FromSet : CompositeConstraint { }
	public abstract class NotFromSet : CompositeConstraint { }
	public abstract class SpecialOrderedSetType1 : CompositeConstraint { }
	public abstract class SpecialOrderedSetType2 : CompositeConstraint { }
}