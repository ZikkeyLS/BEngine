using System.Xml;

namespace BEngineCore
{
	public enum AssetReaderType
	{
		Directory = 0,
		Archive
	}

	public class AssetReader
	{
		public readonly AssetReaderType Type;
		public readonly List<string> AssetsDirectories = new();
		public readonly List<AssetMetaData> LoadedAssets = new();

		public readonly ModelContext ModelContext;

		private Packer? _packer;

		public AssetReader(string[] assetsDirectories, AssetReaderType type)
		{
			ModelContext = new(this);
			AssetsDirectories.AddRange(assetsDirectories);
			Type = type;

			foreach (string assetsDirectory in assetsDirectories)
				if (Directory.Exists(assetsDirectory) == false)
					Directory.CreateDirectory(assetsDirectory);

			if (Type == AssetReaderType.Archive)
			{
				_packer = new();
			}

			foreach (string assetsDirectory in assetsDirectories)
				LoadAssets(assetsDirectory);
		}

		public bool HasAsset(string path)
		{
			string guid = GetMetaID(path);
			return guid != string.Empty;
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

		public void AddAssetRaw(AssetMetaData asset)
		{
			LoadedAssets.Add(asset);
			string assetPath = asset.GetAssetPath();
			if (assetPath.EndsWith(".obj") || assetPath.EndsWith(".fbx") || assetPath.EndsWith(".gltf"))
			{
				ModelContext.AddGUID(asset);
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

		public string GetMetaPath(string guid)
		{
			AssetMetaData? data = LoadedAssets.Find((asset) => asset.GUID == guid);
			return data != null ? data.Path : string.Empty;
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
	}
}
