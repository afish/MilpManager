using System.Security.Cryptography.X509Certificates;

namespace MilpManager.Abstraction
{
    public interface IVariable
    {
        IMilpManager MilpManager { get; set; }
        Domain Domain { get; }
        string Name { get; }
        double? ConstantValue { get; set; }
    }
}