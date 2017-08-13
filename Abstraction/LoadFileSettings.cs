using System.IO;

namespace MilpManager.Abstraction
{
	public class LoadFileSettings
	{
		public string Path { get; set; }
		public Stream SolverData { get; set; }
	}
}