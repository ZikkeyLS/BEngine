using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace BEngineEditor
{
	public class AssetWorker
	{
		private Project _project;

		private FileWatcher _assetWatcher;
		private Timer _timer;

		private const int MSDelay = 10000;

		private List<AssetData> _loadedAssets = new();

		private string _lastMovedAsset = string.Empty;

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
			List<AssetData> removeData = new List<AssetData>();

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

			_project.Logger.LogMessage(_loadedAssets.Count.ToString());
			for (int i = 0; i < _loadedAssets.Count; i++)
			{
				_project.Logger.LogMessage(_loadedAssets[i].GUID);
			}

		}

		public bool HasAsset(string path)
		{
			string guid = GetMetaID(path);
			return guid != string.Empty;
		}

		public void RemoveAsset(string path)
		{
			if (path.EndsWith(".xml") || _lastMovedAsset == path)
				return;

			string guid = GetMetaID(path);

			if (guid == string.Empty)
				return;

			AssetData? foundAsset = _loadedAssets.Find((asset) => asset.GUID == guid);

			if (foundAsset != null)
			{
				_loadedAssets.Remove(foundAsset);
				File.Delete(path + @".xml");
			}
		}

		public void RenameAsset(string oldPath, string newPath)
		{
			if (File.Exists(newPath) == false)
				return;

			_lastMovedAsset = oldPath;

			try
			{
				File.Move(oldPath + @".xml", newPath + @".xml", true);
			}
			catch
			{

			}
		}

		public void AddAsset(string path)
		{
			if (path.EndsWith(".xml") || File.Exists(path) == false)
				return;

			AssetData? asset = AssetData.Load(path);
			if (asset != null)
				_loadedAssets.Add(asset);
		}

		private void LoadAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".xml") == false && file.EndsWith(".csproj") == false && HasAsset(file))
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
			if (path.EndsWith(".xml") || _lastMovedAsset == path || File.Exists(path) == false || HasAsset(path))
				return;

			AssetData asset = new AssetData(GenerateID());
			asset.Save(path);
			_loadedAssets.Add(asset);
		}

		private void CreateSearchedAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".xml") == false && file.EndsWith(".csproj") == false)
				{
					CreateAsset(file);
				}

				else if (file.EndsWith(".xml"))
				{
					if (File.Exists(file.Substring(0, file.IndexOf(".xml"))) == false)
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

		private string GetMetaPath(string guid, string directory)
		{
			string result = string.Empty;

			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".xml"))
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

		private string GetPath(string directory, string guid)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".xml"))
				{
					GetMetaID(file);
				}
			}

			foreach (var subDir in Directory.EnumerateDirectories(directory))
			{
				try
				{
					DirectoryInfo subInfo = new DirectoryInfo(subDir);

					if (subInfo.Name != "bin" && subInfo.Name != "obj")
						GetPath(subDir, guid);
				}
				catch
				{

				}
			}

			return string.Empty;
		}

		private string GetMetaID(string path, bool includeXMLEnd = true)
		{
			string xmlEnd = includeXMLEnd ? ".xml" : string.Empty;

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