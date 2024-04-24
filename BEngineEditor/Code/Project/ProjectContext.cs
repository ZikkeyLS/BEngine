namespace BEngineEditor
{
	public class ProjectContext
	{
		public static ProjectContext Instance { get; private set; }

		private const string TemplateProjectDirectory = "ProjectTemplate";

		public string TempProjectPath = "";
		public string TempProjectName = "NewProject";
		public bool SearchingProject = true;

		public bool ValidTempProjectPath => !Directory.Exists(AssembledTempProjectPath);
		public string AssembledTempProjectPath => $@"{TempProjectPath}\{TempProjectName}";
		public bool ProjectLoaded => _currentProject != null;
		public EditorProject CurrentProject => _currentProject;

		private EditorProject _currentProject;

		public EditorWindow Window { get; private set; }

		public ProjectContext(EditorWindow window)
		{
			Instance = this;
			Window = window;
		}

		public void CreateProject()
		{
			Utils.CopyDirectory(TemplateProjectDirectory, AssembledTempProjectPath);
			ProjectBuilder.RemoveTempMarker(AssembledTempProjectPath);

			ProjectBuilder.PrepareProjectStructure(
				AssembledTempProjectPath, TempProjectName,
				Directory.GetCurrentDirectory() + @"\BEngineCore.dll",
				Directory.GetCurrentDirectory() + @"\BEngineScripting.dll");

			LoadProject(AssembledTempProjectPath + @$"\{TempProjectName}.sln");
		}

		public void LoadProject(string slnPath)
		{
			_currentProject?.Settings.Save();
			if (_currentProject?.LoadedScene != null && _currentProject.Runtime == false)
			{
				_currentProject.SaveCurrentScene();
			}

			string name = Path.GetFileNameWithoutExtension(slnPath);
			string directory = Path.GetDirectoryName(slnPath);

			ProjectBuilder.RegenerateProjectStructure(directory, name,
				Directory.GetCurrentDirectory() + @"\BEngineCore.dll",
				Directory.GetCurrentDirectory() + @"\BEngineScripting.dll");
			_currentProject = new EditorProject(name, directory, Window);
			SearchingProject = false;

			Window.Settings.ProjectHistory.Remove(new LastProject(_currentProject.Name, _currentProject.Directory));
			Window.Settings.ProjectHistory.Add(new LastProject(_currentProject.Name, _currentProject.Directory));

			_currentProject.LoadProject();
		}
	}
}
