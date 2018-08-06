using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public enum GoalType
    {
        Minimize,
        Maximize,
        MinimizeMaximum,
        MaximizeMinimum,
        MaximizeMaximum,
        MinimizeMinimum
    }

    public static class GoalTypeMapper
    {
        public static Type Map(GoalType type)
        {
            switch (type)
            {
                case GoalType.Minimize:
                    return typeof(Minimize);
                case GoalType.Maximize:
                    return typeof(Maximize);
                case GoalType.MinimizeMaximum:
                    return typeof(MinimizeMaximum);
                case GoalType.MaximizeMinimum:
                    return typeof(MaximizeMinimum);
                case GoalType.MaximizeMaximum:
                    return typeof(MaximizeMaximum);
                case GoalType.MinimizeMinimum:
                    return typeof(MinimizeMinimum);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}