
namespace BEngineEditor
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

			EditorWindow window = new EditorWindow("BEngine - Editor");
			window.Run();
		}
	}
}
