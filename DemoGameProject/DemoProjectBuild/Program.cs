namespace DemoProjectBuild
{
	internal class Program
	{
		static void Main(string[] args)
		{
			BEngineCore.Window.Initialize(AppDomain.CurrentDomain.BaseDirectory + "DemoProjectAssembly.dll");
			BEngineCore.Window window = new BEngineCore.Window();
			window.Run();
		}
	}
}
