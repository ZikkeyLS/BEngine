namespace BEngineEditor
{
	public class AssetWorker
	{
		private Project _project;

		private Random _random;

		private Timer _timer;
		private bool _verify = true;

		private const int MSDelay = 500;

		public AssetWorker(Project project)
		{
			_project = project;
			_random = new Random();

			if (Directory.Exists(project.AssetsDirectory) == false)
				Directory.CreateDirectory(project.AssetsDirectory);

			_timer = new Timer((a) => { Iterate(); }, null, 0, MSDelay);
		}

		private void Iterate()
		{
			if (_verify)
			{
				VerifyFiles();
			}
			else
			{
				TryFindLostAssets();
			}

			_verify = !_verify;

			FindNewFiles();
		}

		private void FindNewFiles()
		{
			SearchNewFiles(_project.AssetsDirectory);
			SearchNewFiles(_project.ProjectAssemblyDirectory);
		}

		private void SearchNewFiles(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory).Where(m => !HasAsset(m)))
			{
				if (file.Contains(".csproj") == false)
					AddAsset(file);
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				DirectoryInfo subInfo = new DirectoryInfo(subDir);

				if (subInfo.Name != "bin" && subInfo.Name != "obj")
					SearchNewFiles(subDir);
			}
		}

		private bool HasAsset(string path)
		{
			for (int i = 0; i < _project.Settings.Assets.Count; i++)
			{
				if (_project.Settings.Assets[i].Path == path)
					return true;
			}

			return false;
		}

		public void VerifyFiles()
		{
			List<AssetData> lostAssets = new List<AssetData>();

			for (int i = 0; i < _project.Settings.Assets.Count; i++)
			{
				AssetData current = _project.Settings.Assets[i];

				if (File.Exists(current.Path) == false)
				{
					_project.Settings.LostAssets.Add(current);
					lostAssets.Add(current);
				}
			}

			for (int i = 0; i < lostAssets.Count; i++)
			{
				_project.Settings.Assets.Remove(lostAssets[i]);
			}

			_project.Settings.Save();
		}

		public void TryFindLostAssets()
		{
			List<AssetData> assetsRestored = new List<AssetData>();

			for (int i = 0; i < _project.Settings.LostAssets.Count; i++)
			{
				AssetData current = _project.Settings.LostAssets[i];

				IEnumerable<string> result = SearchAccessibleFiles(_project.ProjectAssemblyDirectory, Path.GetFileName(current.Path))
					.Union(SearchAccessibleFiles(_project.AssetsDirectory, Path.GetFileName(current.Path)));

				if (result.Count() == 0)
					continue;

				if (result.Count() >= 1)
				{
					string warning = "There is more than one instance of lost file. First will be applied!";
					foreach (string item in result)
						warning += "\n" + item;

					_project.Logger.LogWarning(warning);
				}

				assetsRestored.Add(new AssetData(result.ToList()[0], current.InternalID));
			}

			for (int i = 0; i < assetsRestored.Count; i++)
			{
				AssetData current = assetsRestored[i];

				AssetData? potentialExistingCopy = _project.Settings.Assets.Find((asset) => asset.Path == current.Path);
				if (potentialExistingCopy != null)
				{
					potentialExistingCopy.InternalID = current.InternalID;
				}
				else
				{
					_project.Settings.Assets.Add(current);
				}
			}

			_project.Settings.LostAssets.Clear();
			_project.Settings.Save();
		}

		private IEnumerable<string> SearchAccessibleFiles(string root, string name)
		{
			var files = new List<string>();

			foreach (var file in Directory.EnumerateFiles(root).Where(m => m.Contains(name)))
			{
				if (file.Contains(".csproj") == false)
					files.Add(file);
			}
			foreach (var subDir in Directory.EnumerateDirectories(root))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						files.AddRange(SearchAccessibleFiles(subDir, name));
				}
				catch (UnauthorizedAccessException ex)
				{
					// ...
				}
			}

			return files;
		}

		public void RenameAsset(string oldPath, string newPath)
		{
			AssetData? data = _project.Settings.Assets.Find((asset) => asset.Path == oldPath);

			if (data != null)
			{
				data.Path = newPath;
			}
		}

		public void AddAsset(string path)
		{
			uint id = GenerateID();
			while (_project.Settings.Assets.Find((asset) => asset.InternalID == id) != null)
			{
				id = GenerateID();
			}
			AssetData asset = new AssetData(path, id);
			_project.Settings.Assets.Add(asset);
			_project.Settings.Save();
		}

		private uint GenerateID()
		{
			uint thirtyBits = (uint)_random.Next(1 << 30);
			uint twoBits = (uint)_random.Next(1 << 2);
			return (thirtyBits << 2) | twoBits;
		}
	}
}