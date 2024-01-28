using BEngineCore;
using System.Diagnostics;
using System.IO;

namespace BEngineEditor
{
	public class Project
	{
		public string Name { get; private set; } = string.Empty;
		public string Path { get; private set; } = string.Empty;
		public bool AssemblyLoaded { get; private set; } = false;

		public DateTime AssemblyBuildStartTime;
		public DateTime AssemblyBuildEndTime;

		public string SolutionPath => $@"{Path}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Path}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Path}\{Name}Build";
		public string ProjectAssemblyPath => $@"{ProjectAssemblyDirectory}\{Name}Assembly.csproj";
		public string AssemblyBinaryPath => ProjectAssemblyDirectory + $@"\bin\Debug\net8.0\{Name}Assembly.dll";
		public bool EditorAssemblyExists => File.Exists(AssemblyBinaryPath);

		private Scripting _scripting = new Scripting();
		private ProjectCompiler _compiler = new ProjectCompiler();
		private AssemblyListener _assemblyListener = new AssemblyListener();

		public List<string> CompileErrors { get; private set; } = new List<string>();
		public List<string> CompileWarnings { get; private set; } = new List<string>();
		public List<string> TempCompileErrors { get; private set; } = new List<string>();
		public List<string> TempCompileWarnings { get; private set; } = new List<string>();

		public Project(string name, string path)
		{
			Name = name;
			Path = path;
		}
	
		private void Console_Exited(object? sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void LoadProjectData()
		{
			CompileScripts();
			_assemblyListener.StartWatchOnScripts(this);
			_assemblyListener.OnScriptsChanged += (e) => CompileScripts();

			// Get files and etc.
		}

		private void CompileScripts()
		{
			AssemblyLoaded = false;
			AssemblyBuildStartTime = DateTime.Now;
			_compiler.CompileScriptAssembly(ProjectAssemblyDirectory, true, OnAssemblyOutput);
		}

		private void OnAssemblyOutput(object? sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				// Console.WriteLine(e.Data);

				string parsedMessage = e.Data.Replace($"[{ProjectAssemblyPath}]", string.Empty);

				if (e.Data.Contains("error") && TempCompileErrors.Contains(parsedMessage) == false)
				{
					TempCompileErrors.Add(parsedMessage);
				}
				else if (e.Data.Contains("warning") && TempCompileWarnings.Contains(parsedMessage) == false)
				{
					TempCompileWarnings.Add(parsedMessage);
				}
			}
			else
			{
				AssemblyBuildEndTime = DateTime.Now;
				OnAssemblyCompleted();
			}
		}

		private void OnAssemblyCompleted()
		{
			CompileWarnings = TempCompileWarnings;
			CompileErrors = TempCompileErrors;
			AssemblyLoaded = true;
			TempCompileErrors = new();
			TempCompileWarnings = new();
		} 
	}
}
