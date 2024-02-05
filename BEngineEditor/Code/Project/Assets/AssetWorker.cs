using System.Xml;

namespace BEngineEditor
{
	public class AssetWorker
	{
		private Project _project;

		private FileWatcher _assetWatcher;
		private Timer _timer;

		private const int MSDelay = 10000;

		private List<AssetMetaData> _loadedAssets = new();

		private string _lastMovedAsset = string.Empty;
		private string _lastCreatedAsset = string.Empty;

		public AssetWorker(Project project)
		{
			_project = project;

			if (Directory.Exists(project.AssetsDirectory) == false)
				Directory.CreateDirectory(project.AssetsDirectory);

			_assetWatcher = new FileWatcher(_project.AssetsDirectory, "*.*");
			_assetWatcher.Created += CreateAsset;
			_assetWatcher.Renamed += RenameAsset;
			_assetWatcher.Deleted += RemoveAsset;

			LoadAssets(_project.AssetsDirectory);

			_timer = new Timer((a) => { UpdateData(); }, null, 0, MSDelay);
		}

		private void UpdateData()
		{
			List<AssetMetaData> removeData = new List<AssetMetaData>();

			for (int i = 0; i < _loadedAssets.Count; i++)
			{
				string path = GetMetaPath(_loadedAssets[i].GUID, _project.AssetsDirectory);

				if (path == string.Empty)
				{
					removeData.Add(_loadedAssets[i]);
				}
			}

			for (int i = 0; i < removeData.Count; i++)
			{
				_loadedAssets.Remove(removeData[i]);
			}

			CreateSearchedAssets(_project.AssetsDirectory);

			//_project.Logger.LogMessage(_loadedAssets.Count.ToString());
			//for (int i = 0; i < _loadedAssets.Count; i++)
			//{
			//	_project.Logger.LogMessage(_loadedAssets[i].GUID);
			//}
		}

		public bool HasAsset(string path)
		{
			string guid = GetMetaID(path);
			return guid != string.Empty;
		}

		public void RemoveAsset(string path)
		{
			if (path.EndsWith(".meta") || _lastMovedAsset == path)
				return;

			string guid = GetMetaID(path);

			if (guid == string.Empty)
				return;

			AssetMetaData? foundAsset = _loadedAssets.Find((asset) => asset.GUID == guid);

			if (foundAsset != null)
			{
				_loadedAssets.Remove(foundAsset);
				File.Delete(path + @".meta");
			}
		}

		public void RenameAsset(string oldPath, string newPath)
		{
			if (File.Exists(newPath) == false)
				return;

			_lastMovedAsset = oldPath;

			try
			{
				if (newPath.EndsWith(".scene"))
				{
					Scene? scene = AssetData.ReadRaw<Scene>(newPath);
					scene.SceneName = Path.GetFileNameWithoutExtension(newPath);
					AssetData.WriteRaw(newPath, scene);
				}

				File.Move(oldPath + @".meta", newPath + @".meta", true);
			}
			catch
			{

			}
		}

		public void AddAsset(string path)
		{
			if (path.EndsWith(".meta") || File.Exists(path) == false)
				return;

			AssetMetaData? asset = AssetMetaData.Load(path);
			if (asset != null)
				_loadedAssets.Add(asset);
		}

		private void LoadAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta") == false && file.EndsWith(".csproj") == false && HasAsset(file))
					AddAsset(file);
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						LoadAssets(subDir);
				}
				catch
				{

				}
			}
		}

		public void CreateAsset(string path)
		{
			if (path.EndsWith(".meta") || _lastMovedAsset == path || File.Exists(path) == false || HasAsset(path))
				return;

			AssetMetaData asset = new AssetMetaData(GenerateID());
			asset.Save(path);
			_loadedAssets.Add(asset);
		}

		private void CreateSearchedAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta") == false && file.EndsWith(".csproj") == false)
				{
					CreateAsset(file);
				}

				else if (file.EndsWith(".meta"))
				{
					if (File.Exists(file.Substring(0, file.LastIndexOf(".meta"))) == false)
						File.Delete(file);
				}
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

		public string GetMetaPath(string guid, string directory)
		{
			string result = string.Empty;

			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta"))
				{
					string metaID = GetMetaID(file, false);

					if (guid == metaID)
						result = file;
				}
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						return result + GetMetaPath(guid, subDir);
				}
				catch
				{

				}
			}

			return result;
		}

		public string GetAssetPath(string guid)
		{
			return GetAssetPath(_project.AssetsDirectory, guid);
		}

		public string GetAssetPath(string directory, string guid)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta"))
				{
					if (GetMetaID(file, false) ==  guid)
						return file.Substring(0, file.LastIndexOf(".meta"));
				}
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						return GetAssetPath(subDir, guid);
				}
				catch
				{

				}
			}

			return string.Empty;
		}

		public string GetMetaID(string path, bool includeXMLEnd = true)
		{
			string xmlEnd = includeXMLEnd ? ".meta" : string.Empty;

			try
			{
				if (File.Exists(path + xmlEnd))
				{
					XmlDocument metaFile = new XmlDocument();
					metaFile.Load(path + xmlEnd);

					XmlElement? xRoot = metaFile.DocumentElement;
					string? value = xRoot?.ChildNodes[0]?.InnerText;

					if (value != null)
					{
						return value;
					}
				}
			}
			catch
			{

			}

			return string.Empty;
		}

		private string GenerateID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}