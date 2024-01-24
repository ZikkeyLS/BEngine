namespace DemoProjectBuild
{
	internal class Program
	{
		static void Main(string[] args)
		{
			BEngineCore.Window.Main(AppDomain.CurrentDomain.BaseDirectory + "DemoProjectAssembly.dll");
		}
	}
}
