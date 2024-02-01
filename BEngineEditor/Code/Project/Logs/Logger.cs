namespace BEngineEditor
{
	public class Logger
	{
		public List<string> MessageLogs { get; private set; } = new();
		public HashSet<string> WarningsLogs { get; private set; } = new();
		public HashSet<string> ErrorsLogs { get; private set; } = new();

		private List<string> _safeMessageLogs = new();
		private HashSet<string> _safeWarningsLogs = new();
		private HashSet<string> _safeErrorsLogs = new();

		public void InsertSafeLogs()
		{
			MessageLogs.AddRange(_safeMessageLogs);
			WarningsLogs.UnionWith(_safeWarningsLogs);
			ErrorsLogs.UnionWith(_safeErrorsLogs);

			_safeMessageLogs = new();
			_safeWarningsLogs = new();
			_safeErrorsLogs = new();
		}

		public void LogMessage(string message)
		{
			_safeMessageLogs.Add(LogFormat(message));
		}

		public void LogWarning(string warning)
		{
			_safeWarningsLogs.Add(LogFormat(warning));
		}

		public void LogError(string error)
		{
			_safeErrorsLogs.Add(LogFormat(error));
		}

		private string LogFormat(string input)
		{
			return $"[{DateTime.Now.ToString("HH:mm:ss")}] {input}";
		}

		public void ClearLogs()
		{
			MessageLogs.Clear();
			WarningsLogs.Clear();
			ErrorsLogs.Clear();
		}
	}
}