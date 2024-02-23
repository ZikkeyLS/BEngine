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
		public readonly string AssetsDirectory;
		public readonly List<AssetMetaData> LoadedAssets = new();

		public readonly ModelContext ModelContext;

		private Packer? _packer;

		public AssetReader(string assetsDirectory, AssetReaderType type)
		{
			ModelContext = new(this);
			AssetsDirectory = assetsDirectory;
			Type = type;

			if (Directory.Exists(assetsDirectory) == false)
				Directory.CreateDirectory(assetsDirectory);

			if (Type == AssetReaderType.Archive)
			{
				_packer = new();
			}

			LoadAssets(AssetsDirectory);
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
				AddAssetRaw(asset);
			}
		}

		public void AddAssetRaw(AssetMetaData asset)
		{
			LoadedAssets.Add(asset);
			string path = GetAssetPath(asset.GUID);
			if (path.EndsWith(".obj") || path.EndsWith(".fbx") || path.EndsWith(".gltf"))
			{
				ModelContext.AddGUID(asset.GUID);
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
			return GetAssetPath(AssetsDirectory, guid);
		}

		public string GetAssetPath(string directory, string guid)
		{
			foreach (var file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".meta"))
				{
					if (GetMetaID(file, false) == guid)
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
