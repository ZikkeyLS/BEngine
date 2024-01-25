
namespace BEngine
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Thread thread = new Thread((threadArg) =>
			{
				Console.WriteLine("Welcome to BEngineEditor!");
			});
			thread.Start();

			BEngineCore.Scripting scripting = new BEngineCore.Scripting();
			scripting.Initialize(@"D:\Projects\CSharp\BEngine\DemoGameProject\DemoProjectAssembly\bin\Release\net8.0\DemoProjectAssembly.dll");

			BEngineCore.EditorWindow window = new BEngineCore.EditorWindow();
			window.Run();
		}
	}
}
