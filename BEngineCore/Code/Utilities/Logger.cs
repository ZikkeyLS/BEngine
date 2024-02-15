namespace BEngineCore
{
	public class Logger
	{
		public static Logger? Main { get; private set; } = null;

		public bool EnableFileLogs = false;
		public string FileLogPath = "RuntimeLogs.txt";

		public List<string> MessageLogs { get; private set; } = new();
		public HashSet<string> WarningsLogs { get; private set; } = new();
		public HashSet<string> ErrorsLogs { get; private set; } = new();

		private List<string> _safeMessageLogs = new();
		private HashSet<string> _safeWarningsLogs = new();
		private HashSet<string> _safeErrorsLogs = new();

		public Logger(bool isMain = false)
		{
			if (isMain)
				Main = this;
		}

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
			string formatted = LogFormat(message);
			_safeMessageLogs.Add(formatted);

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, formatted + "\n");
		}

		public void LogWarning(string warning)
		{
			string formatted = LogFormat(warning);
			_safeWarningsLogs.Add(LogFormat(warning));

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, formatted + "\n");
		}

		public void LogError(string error)
		{
			string formatted = LogFormat(error);
			_safeErrorsLogs.Add(LogFormat(error));

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, formatted + "\n");
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

		public void ClearFileLogs()
		{
			if (EnableFileLogs)
				File.WriteAllText(FileLogPath, string.Empty);
		}
	}
}