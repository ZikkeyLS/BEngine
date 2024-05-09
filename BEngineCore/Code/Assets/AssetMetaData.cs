using System.IO;
using System.Xml.Serialization;

namespace BEngineCore 
{
	public enum AssetType
	{
		File = 0,
		ArchieveFile
	}

	[Serializable]
	public class AssetMetaData
	{
		public string GUID = string.Empty;
		[XmlIgnore] public string Path { get; private set; } = string.Empty;
		[XmlIgnore] public string? AdditionalPath { get; private set; } = null;
		[XmlIgnore] public AssetType AssetType;

		private AssetMetaData()
		{

		}

		public AssetMetaData(string guid, string path, string? additionalPath = null)
		{
			GUID = guid;
			SetPath(path);

			if (additionalPath != null)
			{
				SetAdditionalPath(additionalPath);
				AssetType = AssetType.ArchieveFile;
			}
			else
			{
				AssetType = AssetType.File;
			}
		}

		public void SetPath(string path)
		{
			path = path.Replace("/", "\\");
			Path = path;
		}

		public void SetAdditionalPath(string additionalPath)
		{
			additionalPath = additionalPath.Replace("/", "\\");
			AdditionalPath = additionalPath;
		}

		public string GetMetaPath()
		{
			string result;

			if (AssetType == AssetType.File)
			{
				result = Path;
			}
			else
			{
				if (AdditionalPath != null)
					result = AdditionalPath;
				else
					result = string.Empty;
			}

			return result;
		}

		public string GetAssetPath()
		{
			string result;

			if (AssetType == AssetType.File)
			{
				result = Path;
			}
			else
			{
				if (AdditionalPath != null)
					result = AdditionalPath;
				else
					result = string.Empty;
			}

			int i = result.LastIndexOf(".meta");
			if (i >= 0)
				result = result[..i];
			return result;
		}

		public void Save()
		{
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetMetaData));
				using (FileStream fs = new FileStream(Path, FileMode.Create))
				{
					xmlSerializer.Serialize(fs, this);
				}
			}
			catch
			{

			}
		}

		public static AssetMetaData? Load(string path)
		{
			if (File.Exists(path + @".meta") == false)
				return null;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetMetaData));

			using (FileStream fs = new FileStream(path + @".meta", FileMode.OpenOrCreate))
			{
				AssetMetaData? assetData = xmlSerializer.Deserialize(fs) as AssetMetaData;
				if (assetData != null)
					return assetData;
			}

			return null;
		}

		public static AssetMetaData? Load(Stream fileData)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetMetaData));

			AssetMetaData? assetData = xmlSerializer.Deserialize(fileData) as AssetMetaData;
			if (assetData != null)
				return assetData;

			return null;
		}
	}
}
