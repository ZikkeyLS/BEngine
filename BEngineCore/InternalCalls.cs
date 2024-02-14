
using System.Runtime.InteropServices;



namespace BEngineCore
{


	public class InternalCalls
	{
		#region Logger
		
		public static void LogMessage(string message)
		{
			if (Logger.Main != null)
				Logger.Main.LogMessage(message);
		}

		public static void LogWarning(string warning)
		{
			if (Logger.Main != null)
				Logger.Main.LogWarning(warning);
		}

		public static void LogError(string error)
		{
			if (Logger.Main != null)
				Logger.Main.LogError(error);
		}
		#endregion

	}
}
