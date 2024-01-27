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

		public static void PrepareProjectStructure(string path, string projectName, string coreDllPath, string scriptDllPath)
		{
			// Root
			FileInfo solutionFile = new FileInfo(path + "/Project.sln");
			solutionFile.Rename(projectName + ".sln");
			File.WriteAllText(solutionFile.FullName, File.ReadAllText(solutionFile.FullName)
				.Replace("ProjectAssembly", $"{projectName}Assembly")
				.Replace("ProjectBuild", $"{projectName}Build")
				.Replace($"{projectName}BuildProject", $"{projectName}"));

			// Assembly
			DirectoryInfo assemblyDirectory = new DirectoryInfo(path + "/ProjectAssembly");
			assemblyDirectory.Rename($"{projectName}Assembly");

			FileInfo assemblyFile = new FileInfo(assemblyDirectory.FullName + "/ProjectAssembly.csproj");
			assemblyFile.Rename(projectName + "Assembly.csproj");
			File.WriteAllText(assemblyFile.FullName, File.ReadAllText(assemblyFile.FullName)
				.Replace("BEngineScriptingDLLPath", scriptDllPath));

			// Build
			DirectoryInfo buildDirectory = new DirectoryInfo(path + "/ProjectBuild");
			buildDirectory.Rename($"{projectName}Build");

			FileInfo buildFile = new FileInfo(buildDirectory.FullName + "/Project.csproj");
			buildFile.Rename(projectName + ".csproj");
			File.WriteAllText(buildFile.FullName, File.ReadAllText(buildFile.FullName)
				.Replace("ProjectAssembly", $"{projectName}Assembly")
				.Replace("BEngineCoreDLLPath", coreDllPath));

			FileInfo programFile = new FileInfo(buildDirectory.FullName + "/Program.cs");
			File.WriteAllText(programFile.FullName, File.ReadAllText(programFile.FullName)
				.Replace("ProjectAssembly", $"{projectName}Assembly")
				.Replace("ProjectBuild", $"{projectName}Build"));
		}
	}
}
