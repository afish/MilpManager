namespace MilpManager.Abstraction
{
	public class MilpManagerSettings
	{
		public MilpManagerSettings()
		{
			IntegerWidth = 10;
			Epsilon = 0.000000001;
			StoreDebugExpressions = false;
			CacheConstants = true;
		}

		public int IntegerWidth { get; set; }
		public double Epsilon { get; set; }
		public bool StoreDebugExpressions { get; set; }
		public bool CacheConstants { get; set; }
	}
}