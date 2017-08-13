namespace MilpManager.Abstraction
{
	public interface IModelSaver<in TSettings>
	{
		/// <summary>
		/// Saves created model
		/// </summary>
		/// <param name="settings">Settings describing how to save model</param>
		/// <remarks>Details of model serialization are implementation dependent</remarks>
		void SaveModel(TSettings settings);
	}
}