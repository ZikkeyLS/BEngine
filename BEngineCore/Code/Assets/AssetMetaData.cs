using System.Xml.Serialization;

namespace BEngineCore 
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
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetMetaData));
				using (FileStream fs = new FileStream(path + @".meta", FileMode.Create))
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
