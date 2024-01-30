using System.Xml.Serialization;

namespace BEngineEditor
{

	public class ProjectSettings
	{
		public string BuildOS = ProjectCompiler.Win64;

		public List<AssetData> Assets = new List<AssetData>();
		public List<AssetData> LostAssets = new List<AssetData>();

		private string _settingsFilePath = "ProjectSettings.xml";

		private ProjectSettings()
		{

		}

		public ProjectSettings(Project project)
		{
			UpdateResultPath(project);
		}

		public void UpdateResultPath(Project project)
		{
			_settingsFilePath = $@"{project.Directory}\{project.Name}Settings.xml";
		}

		public void Save()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));

			using (FileStream fs = new FileStream(_settingsFilePath, FileMode.Create))
			{
				xmlSerializer.Serialize(fs, this);
			}
		}

		public ProjectSettings? Load()
		{
			if (File.Exists(_settingsFilePath) == false)
				return null;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));

			using (FileStream fs = new FileStream(_settingsFilePath, FileMode.OpenOrCreate))
			{
				ProjectSettings? loadedSettings = xmlSerializer.Deserialize(fs) as ProjectSettings;
				if (loadedSettings != null)
					return loadedSettings;
			}

			return null;
		}
	}
}
