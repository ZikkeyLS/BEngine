
using System.Text;

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

			assemblyDirectory.CreateSubdirectory("Assets");

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

		public static void RegenerateProjectStructure(string path, string projectName, string coreDllPath, string scriptDllPath)
		{
			// Assembly
			DirectoryInfo assemblyDirectory = new DirectoryInfo($"{path}/{projectName}Assembly");
			FileInfo assemblyFile = new FileInfo($"{assemblyDirectory.FullName}/{projectName}Assembly.csproj");

			string[] assemblyLines = File.ReadAllLines(assemblyFile.FullName);
			bool assemblyChanged = false;
			for (int i = 0; i < assemblyLines.Length; i++)
			{
				if (assemblyLines[i].Contains("BEngineScripting"))
				{
					i += 1;
					string parsedAssemblyName = assemblyLines[i].Replace("<HintPath>", "")
						.Replace("</HintPath>", "");
					RemoveAllStartUpSpaces(ref parsedAssemblyName);

					if (parsedAssemblyName != scriptDllPath)
					{
						assemblyLines[i] = $"\t\t\t<HintPath>{scriptDllPath}</HintPath>";
						assemblyChanged = true;
						break;
					}
				}
			}

			if (assemblyChanged)
			{
				File.WriteAllLines(assemblyFile.FullName, assemblyLines);
			}		

			// Build
			DirectoryInfo buildDirectory = new DirectoryInfo($"{path}/{projectName}Build");
			FileInfo buildFile = new FileInfo($"{buildDirectory.FullName}/{projectName}.csproj");

			string[] buildLines = File.ReadAllLines(buildFile.FullName);
			bool buildChanged = false;
			for (int i = 0; i < buildLines.Length; i++)
			{
				if (buildLines[i].Contains("BEngineCore"))
				{
					i += 1;
					string parsedCoreName = buildLines[i].Replace("<HintPath>", "")
						.Replace("</HintPath>", "");
					RemoveAllStartUpSpaces(ref parsedCoreName);

					if (parsedCoreName != coreDllPath)
					{
						buildLines[i] = $"\t\t\t<HintPath>{coreDllPath}</HintPath>";
						buildChanged = true;
						break;
					}
				}
			}

			if (buildChanged)
			{
				File.WriteAllLines(buildFile.FullName, buildLines);
			}
		}

		private static void RemoveAllStartUpSpaces(ref string data)
		{
			bool spaces = true;
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				if (spaces)
				{
					if (data[i] != ' ') 
					{
						spaces = false;
					}
				}
				else
				{
					builder.Append(data[i]);
				}
			}

			data = builder.ToString();
		}
	}
}
