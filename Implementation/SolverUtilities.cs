using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation
{
    internal static class SolverUtilities
    {
        internal static string FormatUnsupportedMessage(object type, params IVariable[] arguments)
        {
            return $"Operation {type} with supplied variables [{string.Join(", ", arguments.Select(a => a.ToString()))}] not supported";
        }
        internal static string FormatUnsupportedMessage(object type, object parameters, params IVariable[] arguments)
        {
            return $"Operation {type} with supplied variables [{string.Join(", ", arguments.Select(v => v.Domain.ToString()).ToArray())}] with parameters {parameters} not supported";
        }
    }
}