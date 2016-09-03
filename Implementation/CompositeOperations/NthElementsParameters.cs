using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class NthElementsParameters : ICompositeOperationParameters
    {
        public IVariable[] Indexes { get; set; }
    }
}