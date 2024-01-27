using BEngineCore;
using System.Diagnostics;

namespace BEngineEditor
{
	public class Project
	{
		public string Name { get; private set; } = string.Empty;
		public string Path { get; private set; } = string.Empty;

		public string SolutionPath => $@"{Path}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Path}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Path}\{Name}Build";
		public string ProjectAssemblyPath => ProjectAssemblyDirectory + $@"\bin\Debug\net8.0\{Name}Assembly.dll";
		public bool EditorAssemblyExists => File.Exists(ProjectAssemblyPath);

		private Scripting _scripting = new Scripting();
		private ProjectCompiler _compiler = new ProjectCompiler();

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

			// Get files and etc.
		}

		private void CompileScripts()
		{
			_compiler.CompileScriptAssembly(ProjectAssemblyDirectory, true, CompileAssemblyReady);
		}

		private void CompileAssemblyReady(object? sender, EventArgs e)
		{
			Console.WriteLine("Assembly compiled sucessfully!");

			_scripting.ReadScriptAssembly(ProjectAssemblyPath);
			StartWatchOnScripts();
		}

		private void StartWatchOnScripts()
		{
			FileSystemWatcher scriptsWatcher = new(ProjectAssemblyDirectory, "*.cs");
			scriptsWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
					   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			scriptsWatcher.EnableRaisingEvents = true;
			scriptsWatcher.Changed += (e, a) => CompileScripts();
			scriptsWatcher.Renamed += (e, a) => CompileScripts();
			scriptsWatcher.Deleted += (e, a) => CompileScripts();
		}
	}
}
