using System.Collections.Generic;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class CountingSortParameters : ICompositeOperationParameters
    {
         public IEnumerable<int> Values { get; set; }
    }
}