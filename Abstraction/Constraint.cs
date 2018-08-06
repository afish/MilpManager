namespace MilpManager.Abstraction
{
	public abstract class Constraint { }
	public abstract class Equal : Constraint { }
	public abstract class GreaterOrEqual : Constraint { }
	public abstract class GreaterThan : Constraint { }
	public abstract class LessOrEqual : Constraint { }
	public abstract class LessThan : Constraint { }
	public abstract class MultipleOf : Constraint { }
	public abstract class NotEqual : Constraint { }
}