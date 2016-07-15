using MilpManager.Abstraction;

namespace MilpManager.Implementation
{
    public static class SolverUtilities
    {
        public static string FormatUnsupportedMessage(object type, params IVariable[] arguments)
        {
            return $"Operation {type} with supplied variables [{string.Join(", ", (object[]) arguments)}] not supported";
        }
    }
}