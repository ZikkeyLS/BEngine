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
	internal class ProjectContext
	{
		private const string TemplateProjectDirectory = "ProjectTemplate";

		public string TempProjectPath = "";
		public string TempProjectName = "NewProject";
		public bool ValidTempProjectPath => !Directory.Exists(AssembledTempProjectPath);
		public string AssembledTempProjectPath => $@"{TempProjectPath}\{TempProjectName}";

		private Project _currentProject;

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
			Console.WriteLine(_currentProject.Name + " " + _currentProject.Path);
			Utils.OpenWithDefaultProgram(slnPath);
		}
	}
}
