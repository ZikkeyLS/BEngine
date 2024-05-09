using BEngine;
using BEngineCore;
using System.Text.Json.Serialization;

namespace BEngineEditor
{
	public enum IDE : byte
	{
		VisualStudio = 0,
		VisualStudioCode = 1
	}

	public class ProjectSettings
	{
		public string BuildOS { get; set; } = ProjectCompiler.Win64;
		public string LastOpenedSceneID { get; set; } = string.Empty;
		public IDE IDE { get; set; } = IDE.VisualStudio;
		public ProjectRuntimeInfo ProjectRuntimeInfo { get; set; } = new();

		[JsonIgnore] private string _settingsFilePath = "ProjectSettings.json";

		public ProjectSettings()
		{

		}

		public ProjectSettings(EditorProject project)
		{
			UpdateResultPath(project);
		}

		public void UpdateResultPath(EditorProject project)
		{
			_settingsFilePath = $@"{project.Directory}\{project.Name}Settings.json";
		}

		public void Save()
		{
			File.WriteAllText(_settingsFilePath, JsonUtils.Serialize(this));
		}

		public ProjectSettings? Load()
		{
			if (File.Exists(_settingsFilePath) == false)
				return null;

			ProjectSettings? loadedSettings = JsonUtils.Deserialize<ProjectSettings>(File.ReadAllText(_settingsFilePath));
			if (loadedSettings != null)
				return loadedSettings;

			return null;
		}
	}
}
