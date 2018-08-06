namespace MilpManager.Abstraction
{
	public abstract class Goal { }
	
	public abstract class Minimize : Goal { }
	public abstract class Maximize : Goal {}
	public abstract class MinimizeMaximum : Goal { }
	public abstract class MaximizeMinimum : Goal { }
	public abstract class MaximizeMaximum : Goal { }
	public abstract class MinimizeMinimum : Goal { }
}