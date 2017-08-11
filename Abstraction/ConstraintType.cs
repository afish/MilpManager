namespace MilpManager.Abstraction
{
	public abstract class ConstraintType { }
	public abstract class Equal : ConstraintType { }
	public abstract class GreaterOrEqual : ConstraintType { }
	public abstract class GreaterThan : ConstraintType { }
	public abstract class LessOrEqual : ConstraintType { }
	public abstract class LessThan : ConstraintType { }
	public abstract class MultipleOf : ConstraintType { }
	public abstract class NotEqual : ConstraintType { }
}