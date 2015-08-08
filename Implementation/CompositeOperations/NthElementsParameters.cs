using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class NthElementsParameters : ICompositeOperationParameters
    {
        public int[] Indexes { get; set; }
    }
}