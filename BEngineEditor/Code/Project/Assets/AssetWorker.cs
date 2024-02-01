namespace BEngineEditor
{
	public class AssetWorker
	{
		private Project _project;

		private Random _random;

		private FileWatcher _assetWatcher;
		private Timer _timer;
		private bool _verify = true;

		private const int MSDelay = 10000;

		public AssetWorker(Project project)
		{
			_project = project;
			_random = new Random();

			if (Directory.Exists(project.AssetsDirectory) == false)
				Directory.CreateDirectory(project.AssetsDirectory);

			_assetWatcher = new FileWatcher(_project.AssetsDirectory, "*.*");
			_assetWatcher.Created += AddAsset;
			_assetWatcher.Renamed += RenameAsset;
			_assetWatcher.Deleted += RemoveAsset;

			_assetWatcher = new FileWatcher(_project.ProjectAssemblyDirectory, "*.*");
			_assetWatcher.Created += AddAsset;
			_assetWatcher.Renamed += RenameAsset;
			_assetWatcher.Deleted += RemoveAsset;

			_timer = new Timer((a) => { UpdateData(); }, null, 0, MSDelay);
		}

		private void UpdateData()
		{
			try
			{
				List<AssetData> removeData = new List<AssetData>();

				for (int i = 0; i < _project.Settings.Assets.Count; i++)
				{
					if (File.Exists(_project.Settings.Assets[i].Path) == false)
					{
						removeData.Add(_project.Settings.Assets[i]);
					}
				}

				for (int i = 0; i < removeData.Count; i++)
				{
					_project.Settings.Assets.Remove(removeData[i]);
				}

				CreateSearchedAssets(_project.AssetsDirectory);
				CreateSearchedAssets(_project.ProjectAssemblyDirectory);

				_project.Settings.Save();
			}
			catch
			{

			}
		}

		public bool HasAsset(string path)
		{
			for (int i = 0; i < _project.Settings.Assets.Count; i++)
			{
				if (_project.Settings.Assets[i].Path == path)
					return true;
			}

			return false;
		}

		public void RemoveAsset(string path)
		{
			AssetData? data = _project.Settings.Assets.Find((asset) => asset.Path == path);

			if (data != null)
			{
				_project.Settings.Assets.Remove(data);
			}
		}

		public void RenameAsset(string oldPath, string newPath)
		{
			if (File.Exists(newPath) == false)
				return;

			AssetData? data = _project.Settings.Assets.Find((asset) => asset.Path == oldPath);

			if (data != null)
			{
				data.Path = newPath;
			}
		}

		public void AddAsset(string path)
		{
			if (File.Exists(path) == false)
				return;

			uint id = GenerateID();
			while (_project.Settings.Assets.Find((asset) => asset.InternalID == id) != null)
			{
				id = GenerateID();
			}
			AssetData asset = new AssetData(path, id);
			_project.Settings.Assets.Add(asset);
		}

		private void CreateSearchedAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (HasAsset(file) == false)
					AddAsset(file);
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						CreateSearchedAssets(subDir);
				}
				catch
				{

				}
			}
		}

		private uint GenerateID()
		{
			uint thirtyBits = (uint)_random.Next(1 << 30);
			uint twoBits = (uint)_random.Next(1 << 2);
			return (thirtyBits << 2) | twoBits;
		}
	}
}