namespace MilpManager.Abstraction
{
	public abstract class GoalType { }
	
	public abstract class Minimize : GoalType { }
	public abstract class Maximize : GoalType {}
	public abstract class MinimizeMaximum : GoalType { }
	public abstract class MaximizeMinimum : GoalType { }
	public abstract class MaximizeMaximum : GoalType { }
	public abstract class MinimizeMinimum : GoalType { }
}