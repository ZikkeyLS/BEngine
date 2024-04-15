using BEngineCore;
using System.IO;

namespace BEngineCore
{
	public class AssetWatcher
	{
		private FileWatcher _assetWatcher;
		private Timer _timer;

		private const int MSDelay = 10000;

		private readonly string _assetsPath;

		public AssetWatcher(string path, AssetWriter writer)
		{
			_assetsPath = path;

			_assetWatcher = new FileWatcher(path, "*.*");
			_assetWatcher.Created += writer.CreateAsset;
			_assetWatcher.Renamed += writer.RenameAsset;
			_assetWatcher.Deleted += writer.RemoveAsset;

			_timer = new Timer((a) => { writer.UpdateData(); }, null, 0, MSDelay);
		}

		public string FindFileWithClass(string className, string? namespaceName = null)
		{
			string result = $"Unknown {namespaceName}.{className}";

			try
			{
				foreach (string file in Directory.EnumerateFiles(_assetsPath, "*.cs", SearchOption.AllDirectories))
				{
					string data = File.ReadAllText(file);
					bool hasNamespace = namespaceName == null ? true : data.Contains($"namespace {namespaceName}"); 

					if (data.Contains($"class {className}") && hasNamespace)
					{
						result = file;
					}
				}
			}
			catch
			{

			}

			return result;
		}
	}
}
