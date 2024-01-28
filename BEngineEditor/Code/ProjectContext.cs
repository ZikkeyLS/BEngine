using BEngineCore;
using BEngineEditor.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	public class ProjectContext
	{
		private const string TemplateProjectDirectory = "ProjectTemplate";

		public string TempProjectPath = "";
		public string TempProjectName = "NewProject";
		public bool SearchingProject = true;

		public bool ValidTempProjectPath => !Directory.Exists(AssembledTempProjectPath);
		public string AssembledTempProjectPath => $@"{TempProjectPath}\{TempProjectName}";
		public bool ProjectLoaded => _currentProject != null;
		public Project CurrentProject => _currentProject;

		private Project _currentProject;

		public EditorWindow Window { get; private set; }

		public ProjectContext(EditorWindow window)
		{
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
			_currentProject = new Project(Path.GetFileNameWithoutExtension(slnPath), Path.GetDirectoryName(slnPath));
			SearchingProject = false;

			_currentProject.LoadProjectData();

			// Utils.OpenWithDefaultProgram(_currentProject.SolutionPath);
			// BEngineCore.Scripting scripting = new BEngineCore.Scripting();
			// scripting.Initialize(@"D:\Projects\CSharp\BEngine\DemoGameProject\DemoProjectAssembly\bin\Release\net8.0\DemoProjectAssembly.dll");
		}
	}
}
