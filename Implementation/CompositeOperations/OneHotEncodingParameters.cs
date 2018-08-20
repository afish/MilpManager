using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class OneHotEncodingParameters : ICompositeOperationParameters
    {
        public uint MaximumValue { get; set; }
    }
}