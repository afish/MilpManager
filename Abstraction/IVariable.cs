namespace MilpManager.Abstraction
{
    public interface IVariable
    {
        IMilpManager MilpManager { get; }
        Domain Domain { get; }
    }
}