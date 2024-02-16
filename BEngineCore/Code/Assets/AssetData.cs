using BEngineCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BEngineCore
{
	public class AssetData
	{
		private string _guid;
		private ProjectAbstraction _project;

		[JsonIgnore]
		public string GUID => _guid;
		[JsonIgnore]
		public ProjectAbstraction Project => _project;

		protected AssetData()
		{

		}

		public AssetData(string guid, ProjectAbstraction project) 
		{
			_guid = guid;
			_project = project;
		}

		public void SetForceID(string guid) => _guid = guid;
		public void SetForceProject(ProjectAbstraction project) => _project = project;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
		public static void CreateTemplate<T>(string path, object[]? args = null) where T : AssetData
		{
			T template = (T)Activator.CreateInstance(typeof(T), args);

			File.WriteAllText(path, JsonSerializer.Serialize(template));
		}

		public static T ReadRaw<T>(string path) where T : AssetData
		{
			T? assetData = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
			if (assetData != null)
			{
				return assetData;
			}

			return null;
		}
		
		public static void WriteRaw<T>(string path, T data) where T : AssetData
		{
			File.WriteAllText(path, JsonSerializer.Serialize(data));
		}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

		public void Save<T>() where T : AssetData
		{
			string path = _project.AssetsReader.GetAssetPath(_guid);

			if (path == string.Empty)
				return;

			File.WriteAllText(path, JsonSerializer.Serialize((T)this));
		}

		public static T? Load<T>(string guid, ProjectAbstraction project) where T : AssetData
		{
			string path = project.AssetsReader.GetAssetPath(guid);

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
