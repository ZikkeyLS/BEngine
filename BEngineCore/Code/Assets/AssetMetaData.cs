using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BEngineCore 
{
	[Serializable]
	public class AssetMetaData
	{
		public string GUID = string.Empty;
		[XmlIgnore] public string Path = string.Empty;

		private AssetMetaData()
		{

		}

		public AssetMetaData(string guid, string path)
		{
			GUID = guid;
			Path = path;
		}

		public string GetAssetPath()
		{
			string result = Path;
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
	}
}
