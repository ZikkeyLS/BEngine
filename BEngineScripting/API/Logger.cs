
namespace BEngine
{
	public class Logger
	{
		public static void LogMessage(string message) => InternalCalls.LogMessage(message);
		public static void LogWarning(string warning) => InternalCalls.LogWarning(warning);
		public static void LogError(string error) => InternalCalls.LogError(error);
	}
}
