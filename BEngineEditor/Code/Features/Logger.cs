namespace BEngineEditor
{
	public class Logger
	{
		public HashSet<string> MessageLogs { get; private set; } = new();
		public HashSet<string> WarningsLogs { get; private set; } = new();
		public HashSet<string> ErrorsLogs { get; private set; } = new();

		private HashSet<string> _safeMessageLogs = new();
		private HashSet<string> _safeWarningsLogs = new();
		private HashSet<string> _safeErrorsLogs = new();

		public void InsertSafeLogs()
		{
			MessageLogs.UnionWith(_safeMessageLogs);
			WarningsLogs.UnionWith(_safeWarningsLogs);
			ErrorsLogs.UnionWith(_safeErrorsLogs);

			_safeMessageLogs = new();
			_safeWarningsLogs = new();
			_safeErrorsLogs = new();
		}

		public void LogMessage(string message)
		{
			_safeMessageLogs.Add(message);
		}

		public void LogWarning(string warning)
		{
			_safeWarningsLogs.Add(warning);
		}

		public void LogError(string error)
		{
			_safeErrorsLogs.Add(error);
		}

		public void ClearLogs()
		{
			MessageLogs.Clear();
			WarningsLogs.Clear();
			ErrorsLogs.Clear();
		}
	}
}