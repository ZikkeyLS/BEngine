using System.Reflection;

namespace BEngine
{
	public class InternalCalls
	{
		public static void Log(string message)
		{
			var dll = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "BEngineCore.dll");
			foreach (Type type in dll.GetExportedTypes())
			{
				if (type.Name == nameof(InternalCalls))
				{
					type.GetMethod("Log")?.Invoke(null, new object[] { message });
				}
			}
		}
	}
}
