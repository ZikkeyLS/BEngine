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

		public Project(string name, string path)
		{
			Name = name;
			Path = path;
		}

		public void CompileScriptAssembly(bool debug = true)
		{
			try
			{
				string mode = debug ? "Debug" : "Release";

				Thread thread = new Thread(() =>
				{
					Process console = new Process();
					console.StartInfo.FileName = "cmd.exe";
					console.EnableRaisingEvents = true;
					console.Exited += CompileAssemblyReady;
					console.StartInfo.RedirectStandardInput = true;
					console.StartInfo.RedirectStandardOutput = true;
					console.StartInfo.CreateNoWindow = true;
					console.StartInfo.UseShellExecute = false;
					console.Start();

					console.StandardInput.WriteLine($"dotnet build {ProjectAssemblyDirectory} -c {mode}");
					console.StandardInput.Flush();
					console.StandardInput.Close();
					console.WaitForExit();
					Console.WriteLine(console.StandardOutput.ReadToEnd());
				});
				thread.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void CompileAssemblyReady(object? sender, EventArgs e)
		{
			Console.WriteLine("Assembly compiled sucessfully!");
		}

		public void CompileBuild(bool debug = false)
		{

		}

		public void LoadProjectData()
		{
			CompileScriptAssembly();

			
			// Get files and etc.
		}
	}
}
