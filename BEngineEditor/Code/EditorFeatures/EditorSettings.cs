using System.Xml.Serialization;

namespace BEngineEditor
{
	[Serializable]
	public struct LastProject
	{
		public string Name = string.Empty;
		public string Directory = string.Empty;

		public string SolutionPath => $@"{Directory}\{Name}.sln";

		public LastProject()
		{

		}

		public LastProject(string name, string directory)
		{
			Name = name;
			Directory = directory;
		}
	}

	public class EditorSettings
	{
		public List<LastProject> ProjectHistory = new();

		private const string SettingsFileName = "EdtiorSettings.xml";

		public EditorSettings() 
		{
		
		}

		public void Save()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(EditorSettings));

			using (FileStream fs = new FileStream(SettingsFileName, FileMode.Create))
			{
				xmlSerializer.Serialize(fs, this);
			}
		}

		public EditorSettings? Load()
		{
			if (File.Exists(SettingsFileName) == false)
				return null;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(EditorSettings));

			using (FileStream fs = new FileStream(SettingsFileName, FileMode.OpenOrCreate))
			{
				EditorSettings? loadedSettings = xmlSerializer.Deserialize(fs) as EditorSettings;
				if (loadedSettings != null)
					return loadedSettings;
			}

			return null;
		}
	}
}
