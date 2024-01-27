
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

			EditorWindow window = new EditorWindow("BEngine - Editor");
			window.Run();
		}
	}
}
