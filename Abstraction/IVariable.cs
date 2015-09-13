using System.Security.Cryptography.X509Certificates;

namespace MilpManager.Abstraction
{
    public interface IVariable
    {
        IMilpManager MilpManager { get; set; }
        Domain Domain { get; set; }
        string Name { get; set; }
        double? ConstantValue { get; set; }
        string Expression { get; set; }
    }
}