namespace MilpManager.Abstraction
{
    public interface IVariable
    {
        /// <summary>
        /// Solver used to create variable
        /// </summary>
        IMilpManager MilpManager { get; set; }

        /// <summary>
        /// Domain of a variable
        /// </summary>
        Domain Domain { get; set; }

        /// <summary>
        /// Name of a variable
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Precalculated constant value
        /// </summary>
        /// <remarks>This value is precalculated by calculators, it doesn't mean that this will be variable's value after solving model</remarks>
        double? ConstantValue { get; set; }

        /// <summary>
        /// Textual representation of a variable
        /// </summary>
        string Expression { get; set; }
    }
}