using System.Text.Json;
using System.Text.Json.Serialization;

namespace BEngineEditor
{
	public class AssetData
	{
		private string _guid;
		private Project _project;

		[JsonIgnore]
		public string GUID => _guid;
		[JsonIgnore]
		public Project Project => _project;

		protected AssetData()
		{

		}

		public AssetData(Project project, string guid) 
		{
			_project = project;
			_guid = guid;
		}

		public void SetForceID(string guid) => _guid = guid;
		public void SetForceProject(Project project) => _project = project;


#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
		public static void CreateTemplate<T>(string path, object[]? args = null) where T : AssetData
		{
			T template = (T)Activator.CreateInstance(typeof(T), args);

			File.WriteAllText(path, JsonSerializer.Serialize(template));
		}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

		public void Save<T>() where T : AssetData
		{
			string path = _project.AssetWorker.GetAssetPath(_guid);

			if (path == string.Empty)
				return;

			File.WriteAllText(path, JsonSerializer.Serialize((T)this));
		}

		public static T? Load<T>(string guid, Project project) where T : AssetData
		{
			string path = project.AssetWorker.GetAssetPath(guid);

			if (path == string.Empty)
				return null;

			T? assetData = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
			if (assetData != null)
			{
				assetData.SetForceID(guid);
				assetData.SetForceProject(project);
				return assetData;
			}

			return null;
		}
	}
}
