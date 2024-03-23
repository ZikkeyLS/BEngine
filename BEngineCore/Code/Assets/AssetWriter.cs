
namespace BEngineCore
{
	public class AssetWriter
	{
		private AssetReader _assetReader;

		private readonly string _path;
		private string _lastMovedAsset = string.Empty;

		public AssetWriter(string path, AssetReader reader)
		{
			_assetReader = reader;
			_path = path;
		}

		public void UpdateData()
		{
			List<AssetMetaData> removeData = new List<AssetMetaData>();

			for (int i = 0; i < _assetReader.LoadedAssets.Count; i++)
			{
				string path = _assetReader.GetMetaPath(_assetReader.LoadedAssets[i].GUID);

				if (path == string.Empty)
				{
					removeData.Add(_assetReader.LoadedAssets[i]);
				}
			}

			for (int i = 0; i < removeData.Count; i++)
			{
				_assetReader.LoadedAssets.Remove(removeData[i]);
			}

			CreateSearchedAssets(_path);

			//_project.Logger.LogMessage(_loadedAssets.Count.ToString());
			//for (int i = 0; i < _loadedAssets.Count; i++)
			//{
			//	_project.Logger.LogMessage(_loadedAssets[i].GUID);
			//}
		}

		public void RemoveAsset(string path)
		{
			if (path.EndsWith(".meta") || _lastMovedAsset == path)
				return;

			string guid = _assetReader.GetMetaID(path);

			if (guid == string.Empty)
				return;

			AssetMetaData? foundAsset = _assetReader.LoadedAssets.Find((asset) => asset.GUID == guid);

			if (foundAsset != null)
			{
				if (path.EndsWith(".obj") || path.EndsWith(".fbx") || path.EndsWith(".gltf"))
				{
					_assetReader.ModelContext.RemoveGUID(guid);
				}

				_assetReader.LoadedAssets.Remove(foundAsset);
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
				else if (newPath.EndsWith(".obj") || newPath.EndsWith(".fbx") || newPath.EndsWith(".gltf"))
				{
					string guid = _assetReader.GetMetaID(oldPath + ".meta");
					if (guid != string.Empty)
						_assetReader.ModelContext.GUIDMoved(guid, newPath);
				}

				if (File.Exists(oldPath + ".meta"))
				{
					string guid = _assetReader.GetMetaID(oldPath + ".meta");
					AssetMetaData? assetMeta = _assetReader.LoadedAssets.Find((asset) => asset.GUID == guid);
					if (assetMeta != null)
					{
						assetMeta.Path = newPath + ".meta";
						assetMeta.Save();
					}

					File.Move(oldPath + ".meta", newPath + ".meta", true);
				}
			}
			catch
			{

			}
		}

		public void CreateAsset(string path)
		{
			if (path.EndsWith(".meta") || _lastMovedAsset == path || File.Exists(path) == false || _assetReader.HasAsset(path))
				return;

			if (File.Exists(path + ".meta"))
			{
				_assetReader.AddAsset(path);
			}
			else
			{
				AssetMetaData asset = new AssetMetaData(GenerateID(), path + ".meta");
				asset.Save();
				_assetReader.AddAssetRaw(asset);
			}
		}

		private void CreateSearchedAssets(string directory)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta") == false && file.EndsWith(".csproj") == false && File.Exists(file + ".meta") == false)
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

		public string GetAssetPath(string guid)
		{
			return _assetReader.GetAssetPath(guid);
		}

		private string GenerateID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
