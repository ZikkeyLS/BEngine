using BEngineCore;

namespace BEngineEditor
{
	public class Project
	{
		private Scripting _scripting = new Scripting();
		private ProjectCompiler _compiler = new ProjectCompiler();
		private AssemblyListener _assemblyListener = new AssemblyListener();
		private Logger _logger = new Logger();

		public string Name { get; private set; } = string.Empty;
		public string Directory { get; private set; } = string.Empty;

		public ProjectCompiler Compiler => _compiler;
		public Logger Logger => _logger;

		public string SolutionPath => $@"{Directory}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Directory}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Directory}\{Name}Build";
		public string ProjectBuildBinaryDirectory => $@"{ProjectBuildDirectory}\bin\Release\net8.0\";
		public string ProjectAssemblyPath => $@"{ProjectAssemblyDirectory}\{Name}Assembly.csproj";
		public string AssemblyBinaryPath =>  $@"{ProjectAssemblyDirectory}\bin\Debug\net8.0\{Name}Assembly.dll";
		public bool EditorAssemblyExists => File.Exists(AssemblyBinaryPath);

		public Project(string name, string path)
		{
			Name = name;
			Directory = path;
		}

		public void LoadProjectData()
		{
			_compiler.Initialize(this);
			_compiler.CompileScripts();
			_assemblyListener.StartWatchOnScripts(this);
			_assemblyListener.OnScriptsChanged += (e) => _compiler.CompileScripts();

			// Get files and etc.
		}
	}
}
