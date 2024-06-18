using System.Text;
using System.Xml;

namespace BEngineCore
{
	public class AssetReader
	{
		public readonly List<string> AssetsDirectories = new();
		public readonly List<string> PackerDirectories = new();

		public readonly List<AssetMetaData> LoadedAssets = new();

		public readonly ModelContext ModelContext;
		public readonly ShaderContext ShaderContext;
		public readonly Dictionary<string, Scene> SceneContext = new();

		private Packer? _packer;

		public Packer? Packer => _packer;

		public AssetReader(string[] assetsDirectories, string[] packerDirectories)
		{
			ModelContext = new(this);
			ShaderContext = new(this);

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
				asset.SetPath(path + ".meta");
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
				asset.SetPath(archivePath);
				asset.SetAdditionalPath(relativePath + ".meta");
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
				Stream? stream = GetAssetStream(asset);

				if (stream == null)
					return;

				Scene? scene = AssetData.ReadRaw<Scene>(stream);
				stream.Close();

				if (scene != null)
				{
					scene.SetForceProject(ProjectAbstraction.LoadedProject);
					scene.SetForceID(asset.GUID);
					SceneContext.TryAdd(asset.GUID, scene);
				}
			}
			else if (assetPath.EndsWith(".shader"))
			{
				string? shaderData = GetAssetText(asset);
				if (shaderData != null)
				{
					ShaderContext.Add(asset.GUID, shaderData);
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

		public AssetMetaData? GetAssetByPath(string path)
		{
			var loadedAssetsCopy = new List<AssetMetaData>();
			loadedAssetsCopy.AddRange(LoadedAssets);
			foreach (var asset in loadedAssetsCopy)
			{
				path = path.Replace("/", "\\");
				string assetPath = asset.GetAssetPath();
				if (assetPath.Contains(path))
				{
					return asset;
				}
			}

			return null;
		}

		public string? GetAssetText(AssetMetaData asset)
		{
			if (asset.AssetType == AssetType.File)
			{
				string path = asset.GetAssetPath();
				return File.ReadAllText(path);
			}
			else
			{
				if (Packer == null)
					return null;

				Stream? data = Packer.ReadFile(asset.Path, asset.GetAssetPath());
				if (data != null)
				{
					return StreamToString(data);
				}
			}

			return null;
		}

		public byte[] GetAssetBytes(AssetMetaData asset)
		{
			if (asset.AssetType == AssetType.File)
			{
				string path = asset.GetAssetPath();
				return File.ReadAllBytes(path);
			}
			else
			{
				if (Packer == null)
					throw new Exception("Packer isn't initialized!");

				Stream? data = Packer.ReadFile(asset.Path, asset.GetAssetPath());
				if (data != null)
				{
					return StreamToBytes(data);
				}
			}

			return Array.Empty<byte>();
		}

		public string[]? GetAssetLines(AssetMetaData asset)
		{
			if (asset.AssetType == AssetType.File)
			{
				string path = asset.GetAssetPath();
				return File.ReadAllLines(path);
			}
			else
			{
				if (Packer == null)
					return null;

				Stream? data = Packer.ReadFile(asset.Path, asset.GetAssetPath());
				if (data != null)
				{
					List<string> result = new List<string>();
					using (var streamReader = new StreamReader(data, Encoding.UTF8, true, 4096))
					{
						string? line;
						while ((line = streamReader.ReadLine()) != null)
						{
							result.Add(line);
						}
					}
					return result.ToArray();
				}
			}

			return null;
		}

		public Stream? GetAssetStream(AssetMetaData asset)
		{
			if (asset.AssetType == AssetType.File)
			{
				string path = asset.GetAssetPath();
				return File.Open(path, FileMode.Open);
			}
			else
			{
				if (Packer == null)
					return null;

				Stream? data = Packer.ReadFile(asset.Path, asset.GetAssetPath());
				if (data != null)
				{
					return data;
				}
			}

			return null;
		}

		private static string StreamToString(Stream stream)
		{
			using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
			{
				return reader.ReadToEnd();
			}
		}

		private static byte[] StreamToBytes(Stream stream)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				return ms.ToArray();
			}
		}
	}
}
