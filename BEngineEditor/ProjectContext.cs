using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal class ProjectContext
	{
		private const string TemplateProjectDirectory = "ProjectTemplate";

		public string TempProjectPath = "";
		public string TempProjectName = "NewProject";
		public bool ValidTempProjectPath { get; private set; } = true;
		public string AssembledTempProjectPath => $@"{TempProjectPath}\{TempProjectName}";

		private Project _currentProject;

		public bool IsCreationPathValid(string directory)
		{
			return !Directory.Exists(directory);
		}

		public void UpdateCreationPathValid(string directory)
		{
			ValidTempProjectPath = !Directory.Exists(directory);
		}

		public void CreateProject() 
		{
			Utils.CopyDirectory(TemplateProjectDirectory, AssembledTempProjectPath);
		}

		public void LoadProject()
		{

		}
	}
}
