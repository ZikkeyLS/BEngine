using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

[assembly: InternalsVisibleTo("BEngineCore", AllInternalsVisible = true)]

namespace BEngine
{
	internal class InternalCalls
	{
		private static Type _internalCalls;

		public static void LoadInternalCallsAPI()
		{
			Type? internalCallsAPI = GetTypeByName("BEngineCore.InternalCalls");
			if (internalCallsAPI != null)
			{
				_internalCalls = internalCallsAPI;
				LoadLoggerAssembly();
			}
		}

		private static Type? GetTypeByName(string name)
		{
			return
				AppDomain.CurrentDomain.GetAssemblies()
					.Reverse()
					.Select(assembly => assembly.GetType(name))
					.FirstOrDefault(t => t != null)
				// Safely delete the following part
				// if you do not want fall back to first partial result
				??
				AppDomain.CurrentDomain.GetAssemblies()
					.Reverse()
					.SelectMany(assembly => assembly.GetTypes())
					.FirstOrDefault(t => t.Name.Contains(name));
		}

		private static MethodInfo? GetMethod(string name) => _internalCalls.GetMethod(name);

		#region Logger
		private static MethodInfo? _logMessage;
		private static MethodInfo? _logWarning;
		private static MethodInfo? _logError;

		private static void LoadLoggerAssembly()
		{
			_logMessage = GetMethod("LogMessage");
			_logWarning = GetMethod("LogWarning");
			_logError = GetMethod("LogErorr");
		}

		public static void LogMessage(string message)
		{
			_logMessage?.Invoke(null, new object[] { message });
		}

		public static void LogWarning(string warning)
		{
			_logWarning?.Invoke(null, new object[] { warning });
		}

		public static void LogError(string error)
		{
			_logError?.Invoke(null, new object[] { error });
		}
		#endregion
	}
}
