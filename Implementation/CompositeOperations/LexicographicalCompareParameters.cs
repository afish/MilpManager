using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class LexicographicalCompareParameters : ICompositeOperationParameters
    {
        public IVariable[] Pattern { get; set;  }
    }
}