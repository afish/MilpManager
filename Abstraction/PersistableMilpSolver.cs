using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MilpManager.Abstraction
{
	public abstract class PersistableMilpSolver : MilpSolver, IModelSaver<SaveFileSettings>, IModelManager<LoadFileSettings>
	{
	    protected PersistableMilpSolver(MilpManagerSettings settings) : base(settings)
	    {
	    }

        public virtual void SaveSolverData(Stream solverData)
		{
			var objectsToSerialize = GetObjectsToSerialize();
			new BinaryFormatter().Serialize(solverData, new[] { Variables, objectsToSerialize});
		}

		public void LoadModel(LoadFileSettings settings)
		{
			InternalLoadModelFromFile(settings.Path);
			var deserialized = (object[])new BinaryFormatter().Deserialize(settings.SolverData);
			Variables = (IDictionary<string, IVariable>)deserialized[0];
			VariableIndex = Variables.Count;
			foreach (var variable in Variables)
			{
				variable.Value.MilpManager = this;
			}
			InternalDeserialize(deserialized.Length > 1 ? deserialized[1] : null);
		}

		protected abstract object GetObjectsToSerialize();
		protected abstract void InternalDeserialize(object o);
		protected abstract void InternalLoadModelFromFile(string modelPath);
		public abstract void SaveModel(SaveFileSettings settings);
	}
}