using System.IO;
using System.Xml;

namespace BEngineCore
{
	public class AssetReader
	{
		public readonly List<string> AssetsDirectories = new();
		public readonly List<string> PackerDirectories = new();

		public readonly List<AssetMetaData> LoadedAssets = new();

		public readonly ModelContext ModelContext;
		public readonly Dictionary<string, Scene> SceneContext = new();

		private Packer? _packer;

		public Packer? Packer => _packer;

		public AssetReader(string[] assetsDirectories, string[] packerDirectories)
		{
			ModelContext = new(this);
			AssetsDirectories.AddRange(assetsDirectories);
			PackerDirectories.AddRange(packerDirectories);

			foreach (string assetsDirectory in assetsDirectories)
				if (Directory.Exists(assetsDirectory) == false)
					Directory.CreateDirectory(assetsDirectory);

			if (packerDirectories.Length > 0)
			{
				_packer = new();

				foreach (string packerDirectory in packerDirectories)
				{
					_packer.ReadAllFiles(packerDirectory, (relativePath) => 
					{
						if (relativePath.EndsWith(".meta") == false)
							AddAsset(packerDirectory, relativePath);
					});
				}
			}

			foreach (string assetsDirectory in assetsDirectories)
				LoadAssets(assetsDirectory);
		}

		public bool HasAsset(string path)
		{
			string guid = GetMetaID(path);
			string guidArchieve = string.Empty;

			for (int i = 0; i < PackerDirectories.Count; i++)
			{
				guidArchieve = GetMetaArchieveID(PackerDirectories[i], path);
			}

			return guid != string.Empty || guidArchieve != string.Empty;
		}

		public void AddAsset(string path)
		{
			if (path.EndsWith(".meta") || File.Exists(path) == false)
				return;

			AssetMetaData? asset = AssetMetaData.Load(path);
			if (asset != null)
			{
				asset.Path = path + ".meta";
				AddAssetRaw(asset);
			}
		}

		public void AddAsset(string archivePath, string relativePath)
		{
			if (_packer == null)
				return;

			Stream? data = _packer.ReadFile(archivePath, relativePath + ".meta");
			if (data == null)
				return;

			AssetMetaData? asset = AssetMetaData.Load(data);
			if (asset != null)
			{
				asset.Path = archivePath;
				asset.AdditionalPath = relativePath + ".meta";
				asset.AssetType = AssetType.ArchieveFile;
				AddAssetRaw(asset);
			}
		}

		public void AddAssetRaw(AssetMetaData asset)
		{
			LoadedAssets.Add(asset);
			string assetPath = asset.GetAssetPath();
			if (assetPath.EndsWith(".obj") || assetPath.EndsWith(".fbx") || assetPath.EndsWith(".gltf"))
			{
				ModelContext.AddGUID(asset);
			}
			else if (assetPath.EndsWith(".scene"))
			{
				Scene? scene = null;

				if (asset.AssetType == AssetType.ArchieveFile && Packer != null)
				{
					Stream? sceneData = Packer.ReadFile(asset.Path, assetPath);
					if (sceneData != null)
						scene = AssetData.ReadRaw<Scene>(sceneData);
				}
				else if (asset.AssetType == AssetType.File)
				{
					scene = AssetData.ReadRaw<Scene>(assetPath);
				}

				if (scene != null)
				{
					SceneContext.Add(asset.GUID, scene);
				}
			}
		}

		public void LoadAssets(string directory)
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

		public string GetAssetPath(string guid)
		{
			AssetMetaData? data = LoadedAssets.Find((asset) => asset.GUID == guid);
			if (data != null)
				return data.GetAssetPath();
			return string.Empty;
		}

		public AssetMetaData? GetAsset(string guid)
		{
			return LoadedAssets.Find((asset) => asset.GUID == guid);
		}

		public string GetMetaPath(string guid)
		{
			AssetMetaData? data = LoadedAssets.Find((asset) => asset.GUID == guid);
			return data != null ? data.GetMetaPath() : string.Empty;
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

		public string GetMetaArchieveID(string archievePath, string relativePath, bool includeXMLEnd = true)
		{
			if (_packer == null)
				return string.Empty;

			string xmlEnd = includeXMLEnd ? ".meta" : string.Empty;

			try
			{
				Stream? fileData = _packer.ReadFile(archievePath, relativePath + xmlEnd);

				if (fileData == null)
					return string.Empty;

				XmlDocument metaFile = new XmlDocument();
				metaFile.Load(fileData);

				XmlElement? xRoot = metaFile.DocumentElement;
				string? value = xRoot?.ChildNodes[0]?.InnerText;

				if (value != null)
				{
					return value;
				}
			}
			catch
			{

			}

			return string.Empty;
		}
	}
}
