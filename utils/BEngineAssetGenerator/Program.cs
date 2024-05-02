using BEngineCore;

namespace BEngineAssetGenerator
{
	internal class Program
	{
		private const string GeneratorDirectory = "Assets";

		static void Main(string[] args)
		{
			if (Directory.Exists(GeneratorDirectory) == false)
			{
				Directory.CreateDirectory(GeneratorDirectory);
			}

			AssetWatcher assetWatcher = new AssetWatcher(GeneratorDirectory, 
				new AssetWriter(GeneratorDirectory, new AssetReader([GeneratorDirectory], Array.Empty<string>())));

			Console.WriteLine("Write 'Exit' to stop generation.");

			bool running = true;
			while (running)
			{
				if (Console.ReadLine() == "Exit")
				{
					running = false;
				}			
			}

			Console.WriteLine("Generation ended!");
		}
	}
}
