
using ImGuiNET;

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

			BEngineCore.Window.Initialize(@"D:\Projects\CSharp\BEngine\DemoGameProject\DemoProjectAssembly\bin\Release\net8.0\DemoProjectAssembly.dll");

			BEngineCore.Window window = new BEngineCore.Window();
			window.OnRenderUI += () =>
			{
				ImGui.DockSpaceOverViewport();
				ImGui.ShowDemoWindow();
			};
			window.Run();
		}
	}
}
