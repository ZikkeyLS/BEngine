using BEngine;
using System.Text.Json;

namespace BEngineEditor
{
	public struct LastProject
	{
		public string Name { get; set; } = string.Empty;
		public string Directory { get; set; } = string.Empty;

		public string SolutionPath => $@"{Directory}\{Name}.sln";

		public LastProject() { }

		public LastProject(string name, string directory)
		{
			Name = name;
			Directory = directory;
		}
	}

	public class EditorSettings
	{
		public List<LastProject> ProjectHistory { get; set; } = new();
		public bool BuildSettingsOpened { get; set; } = false;

		private const string SettingsFileName = "EditorSettings.json";

		public EditorSettings()
		{
		
		}

		public void Save()
		{
			File.WriteAllText(SettingsFileName, JsonUtils.Serialize(this));
		}

		public EditorSettings? Load()
		{
			if (File.Exists(SettingsFileName) == false)
				return null;

			EditorSettings? loadedSettings = JsonUtils.Deserialize<EditorSettings>(File.ReadAllText(SettingsFileName));
			if (loadedSettings != null)
				return loadedSettings;

			return null;
		}
	}
}
