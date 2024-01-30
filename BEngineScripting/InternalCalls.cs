using System.Reflection;
using System.Runtime.Loader;

namespace BEngine
{
	public class InternalCalls
	{
		private static Type _internalCalls;

		public static void LoadInternalCallsAPI()
		{
			AssemblyLoadContext context = new AssemblyLoadContext(name: "ReadInternalCalls", isCollectible: true);

			Assembly dll = context.LoadFromAssemblyPath(AppDomain.CurrentDomain.BaseDirectory + "BEngineCore.dll");
			foreach (Type type in dll.GetExportedTypes())
			{
				if (type.Name == nameof(InternalCalls))
					_internalCalls = type;
			}

			context.Unload();
		}

		public static void Log(string message)
		{
			_internalCalls.GetMethod("Log")?.Invoke(null, new object[] { message });
		}
	}
}
