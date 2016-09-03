using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ArraySetParameters :  ICompositeOperationParameters
    {
        public IVariable Index { get; set; }
        public IVariable Value { get; set; }
    }
}