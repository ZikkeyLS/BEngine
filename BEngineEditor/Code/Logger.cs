namespace BEngineEditor
{
	public class Logger
	{
		public HashSet<string> MessageLogs { get; private set; } = new();
		public HashSet<string> WarningsLogs { get; private set; } = new();
		public HashSet<string> ErrorsLogs { get; private set; } = new();

		public void LogMessage(string message)
		{
			MessageLogs.Add(message);
		}

		public void LogWarning(string warning)
		{
			WarningsLogs.Add(warning);
		}

		public void LogError(string error)
		{
			ErrorsLogs.Add(error);
		}

		public void ClearLogs()
		{
			MessageLogs.Clear();
			WarningsLogs.Clear();
			ErrorsLogs.Clear();
		}
	}
}