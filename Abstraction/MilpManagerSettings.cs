namespace MilpManager.Abstraction
{
	public class MilpManagerSettings
	{
		public MilpManagerSettings(int integerWidth, double epsilon = 0.000000001)
		{
			IntegerWidth = integerWidth;
			Epsilon = epsilon;
			StoreDebugExpressions = false;
			CacheConstants = true;
		}

		public int IntegerWidth { get; }
		public double Epsilon { get; }
		public bool StoreDebugExpressions { get; set; }
		public bool CacheConstants { get; set; }
	}
}