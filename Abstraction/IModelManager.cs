using System.IO;

namespace MilpManager.Abstraction
{
	public interface IModelManager<in TSettings>
	{
		/// <summary>
		/// Loads model and solver's internal data so it is possible to resume working on a model
		/// </summary>
		/// <param name="settings">Model settings</param>
		void LoadModel(TSettings settings);

		/// <summary>
		/// Saves solver's internal data
		/// </summary>
		/// <param name="solverData">Stream to store solver's data</param>
		void SaveSolverData(Stream solverData);
	}
}