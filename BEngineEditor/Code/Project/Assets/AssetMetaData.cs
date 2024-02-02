using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BEngineEditor 
{
	[Serializable]
	public class AssetMetaData
	{
		public string GUID = string.Empty;

		private AssetMetaData()
		{

		}

		public AssetMetaData(string guid)
		{
			GUID = guid;
		}

		public void Save(string path)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetMetaData));
			using (FileStream fs = new FileStream(path + @".meta", FileMode.Create))
			{
				xmlSerializer.Serialize(fs, this);
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
			}

			return null;
		}
	}
}
