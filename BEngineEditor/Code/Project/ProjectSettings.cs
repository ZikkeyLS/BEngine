using System.Text.Json;
using System.Text.Json.Serialization;

namespace BEngineEditor
{
	public class ProjectSettings
	{
		public string BuildOS { get; set; } = ProjectCompiler.Win64;
		public string LastOpenedSceneID { get; set; } = string.Empty;

		[JsonIgnore]
		private string _settingsFilePath = "ProjectSettings.json";

		public ProjectSettings()
		{

		}

		public ProjectSettings(Project project)
		{
			UpdateResultPath(project);
		}

		public void UpdateResultPath(Project project)
		{
			_settingsFilePath = $@"{project.Directory}\{project.Name}Settings.json";
		}

		public void Save()
		{
			File.WriteAllText(_settingsFilePath, JsonSerializer.Serialize(this));
		}

		public ProjectSettings? Load()
		{
			if (File.Exists(_settingsFilePath) == false)
				return null;

			ProjectSettings? loadedSettings = JsonSerializer.Deserialize<ProjectSettings>(File.ReadAllText(_settingsFilePath));
			if (loadedSettings != null)
				return loadedSettings;

			return null;
		}
	}
}
