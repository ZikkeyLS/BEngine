using BEngineCore;

namespace DemoProjectBuild
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "Data"));

			Scripting scripting = new Scripting();
			scripting.Initialize(Path.Combine(Directory.GetCurrentDirectory(), "DemoProjectAssembly.dll"));

			Window window = new();
			window.Run();
		}
	}
}
