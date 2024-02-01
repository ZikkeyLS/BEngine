using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BEngineEditor 
{
	[Serializable]
	public class AssetData
	{
		public string GUID = string.Empty;

		private AssetData()
		{

		}

		public AssetData(string guid)
		{
			GUID = guid;
		}

		public void Save(string path)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetData));
			using (FileStream fs = new FileStream(path + @".meta", FileMode.Create))
			{
				xmlSerializer.Serialize(fs, this);
			}
		}

		public static AssetData? Load(string path)
		{
			if (File.Exists(path + @".meta") == false)
				return null;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetData));

			using (FileStream fs = new FileStream(path + @".meta", FileMode.OpenOrCreate))
			{
				AssetData? assetData = xmlSerializer.Deserialize(fs) as AssetData;
				if (assetData != null)
					return assetData;
			}

			return null;
		}
	}
}
