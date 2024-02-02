using System.Xml.Serialization;

namespace BEngineEditor
{
	public class AssetData
	{
		private string _guid;
		private Project _project;

		public string GUID => _guid;
		public Project Project => _project;

		protected AssetData()
		{

		}

		public AssetData(Project project, string guid) 
		{
			_project = project;
			_guid = guid;
		}

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
		public static void CreateTemplate<T>(string path, object[]? args = null) where T : AssetData
		{
			T template = (T)Activator.CreateInstance(typeof(T), args);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				xmlSerializer.Serialize(fs, template);
			}
		}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

		public void Save<T>() where T : AssetData
		{
			string path = _project.AssetWorker.GetAssetPath(_guid);

			if (path == string.Empty)
				return;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				xmlSerializer.Serialize(fs, this);
			}
		}

		public static T? Load<T>(string guid, Project project) where T : AssetData
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			string path = project.AssetWorker.GetAssetPath(guid);

			if (path == string.Empty)
				return null;

			// десериализуем объект
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				object? result = xmlSerializer.Deserialize(fs);
				return (T)result;
			}
		}
	}
}
