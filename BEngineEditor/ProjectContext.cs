using System;
using System.Collections.Generic;
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
			ProjectBuilder.PrepareProjectStructure(AssembledTempProjectPath, TempProjectName);
		}

		public void LoadProject()
		{

		}
	}
}
