using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ArrayGetParameters :  ICompositeOperationParameters
    {
        public IVariable Index { get; set; }
    }
}