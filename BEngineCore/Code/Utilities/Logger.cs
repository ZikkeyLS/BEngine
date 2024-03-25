
namespace BEngineCore
{
	public struct LogData
	{
		public string Time;
		public string Data;

		public override string ToString()
		{
			return Time + " " + Data;
		}
	}

	public class Logger
	{
		public static Logger? Main { get; private set; } = null;

		public bool EnableFileLogs = false;
		public string FileLogPath = "RuntimeLogs.txt";

		public List<LogData> MessageLogs { get; private set; } = new();
		public HashSet<LogData> WarningsLogs { get; private set; } = new();
		public HashSet<LogData> ErrorsLogs { get; private set; } = new();

		private List<LogData> _safeMessageLogs = new();
		private HashSet<LogData> _safeWarningsLogs = new();
		private HashSet<LogData> _safeErrorsLogs = new();

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
			LogData format = new LogData() { Data = message, Time = GetTime() };
			_safeMessageLogs.Add(format);

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, format.ToString() + "\n");
		}

		public void LogWarning(string warning)
		{
			LogData format = new LogData() { Data = warning, Time = GetTime() };
			_safeWarningsLogs.Add(format);

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, format.ToString() + "\n");
		}

		public void LogError(string error)
		{
			LogData format = new LogData() { Data = error, Time = GetTime() };
			_safeErrorsLogs.Add(format);

			if (EnableFileLogs)
				File.AppendAllText(FileLogPath, format.ToString() + "\n");
		}

		private string GetTime()
		{
			return $"[{DateTime.Now.ToString("HH:mm:ss")}]";
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