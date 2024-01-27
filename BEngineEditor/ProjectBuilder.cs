using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal static class ProjectBuilder
	{
		public static void RemoveTempMarker(string path)
		{
			var dir = new DirectoryInfo(path);
			foreach (FileInfo file in dir.GetFiles())
			{
				file.Rename(file.Name.Replace(".temp", ""));
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			foreach (DirectoryInfo subDir in dirs)
			{
				RemoveTempMarker(subDir.FullName);
			}
		}

		public static void PrepareProjectStructure(string path, string projectName)
		{
			// Root
			FileInfo solutionFile = new FileInfo(path + "/Project.sln");
			solutionFile.Rename(projectName + ".sln");

			string solutionFilePath = path + "/" + projectName + ".sln";
			File.WriteAllText(solutionFilePath, File.ReadAllText(solutionFilePath).Replace("ProjectAssembly", $"{projectName}Assembly"));
			File.WriteAllText(solutionFilePath, File.ReadAllText(solutionFilePath).Replace("ProjectBuild", $"{projectName}Build"));

			// Assembly
			DirectoryInfo assemblyDirectory = new DirectoryInfo(path + "/ProjectAssembly");
			assemblyDirectory.Rename($"{projectName}Assembly");


			// Build
			DirectoryInfo buildDirectory = new DirectoryInfo(path + "/ProjectBuild");
			buildDirectory.Rename($"{projectName}Build");

			string projectBuildFilePath = path + "/" + projectName + ".sln";
			File.WriteAllText(projectBuildFilePath, File.ReadAllText(projectBuildFilePath).Replace("ProjectAssembly", $"{projectName}Assembly"));
		}
	}
}
