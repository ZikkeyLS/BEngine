using System.Diagnostics;

namespace BEngineEditor
{
	public class ProjectCompiler
	{
		public bool AssemblyLoaded { get; private set; } = false;
		public bool BuildingGame { get; private set; } = false;

		public DateTime AssemblyBuildStartTime;
		public DateTime AssemblyBuildEndTime;

		public DateTime BuildStartTime;
		public DateTime BuildEndTime;

		public HashSet<string> AssemblyCompileErrors { get; private set; } = new();
		public HashSet<string> AssemblyCompileWarnings { get; private set; } = new();

		private HashSet<string> _assemblyCompileErrors = new();
		private HashSet<string> _assemblyCompileWarnings = new();

		public HashSet<string> BuildCompileErrors { get; private set; } = new();
		public HashSet<string> BuildCompileWarnings { get; private set; } = new();

		private HashSet<string> _buildCompileErrors = new();
		private HashSet<string> _buildCompileWarnings = new();

		private Process _assemblyCompilation;
		private Process _buildCompilation;

		public const string Win64 = "win-x64";
		public const string Linux64 = "linux-x64";
		public const string Osx64 = "osx-x64";

		private bool _runOnBuild = false;
		private Project _project;

		private const int CopyDelayInMS = 5000;

		public ProjectCompiler(Project project)
		{
			_project = project;
		}

		public bool IsCurrentOS(string os)
		{
			return _project.Settings.BuildOS == os;
		}

		private void CompileScriptAssembly(bool debug = true, DataReceivedEventHandler? onOutput = null)
		{
			string mode = debug ? "Debug" : "Release";

			if (_assemblyCompilation != null) {
				_assemblyCompilation.Kill();
				_assemblyCompilation.Close();
			}

			_assemblyCompilation = new Process();
			_assemblyCompilation.StartInfo.FileName = "cmd.exe";
			_assemblyCompilation.EnableRaisingEvents = true;
			_assemblyCompilation.OutputDataReceived += onOutput;
			_assemblyCompilation.StartInfo.RedirectStandardInput = true;
			_assemblyCompilation.StartInfo.RedirectStandardOutput = true;
			_assemblyCompilation.StartInfo.CreateNoWindow = true;
			_assemblyCompilation.StartInfo.UseShellExecute = false;

			_assemblyCompilation.Start();
			_assemblyCompilation.BeginOutputReadLine();

			_assemblyCompilation.StandardInput.WriteLine($"dotnet build {_project.ProjectAssemblyDirectory} -c {mode}");
			_assemblyCompilation.StandardInput.Flush();
			_assemblyCompilation.StandardInput.Close();
		}

		private void CompileBuild(bool debug = false, DataReceivedEventHandler? onOutput = null)
		{
			string mode = debug ? "Debug" : "Release";

			if (_buildCompilation != null)
			{
				_buildCompilation.Kill();
				_buildCompilation.Close();
			}

			_buildCompilation = new Process();
			_buildCompilation.StartInfo.FileName = "cmd.exe";
			_buildCompilation.EnableRaisingEvents = true;
			_buildCompilation.OutputDataReceived += onOutput;
			_buildCompilation.StartInfo.RedirectStandardInput = true;
			_buildCompilation.StartInfo.RedirectStandardOutput = true;
			_buildCompilation.StartInfo.CreateNoWindow = true;
			_buildCompilation.StartInfo.UseShellExecute = false;

			_buildCompilation.Start();
			_buildCompilation.BeginOutputReadLine();

			_buildCompilation.StandardInput.WriteLine($"dotnet build {_project.ProjectBuildDirectory} -c {mode} -r {_project.Settings.BuildOS} -m:2");
			_buildCompilation.StandardInput.Flush();
			_buildCompilation.StandardInput.Close();
		}

		public void CompileScripts()
		{
			AssemblyLoaded = false;
			AssemblyBuildStartTime = DateTime.Now;
			CompileScriptAssembly(true, OnAssemblyOutput);
		}

		public void BuildGame(bool run = false)
		{
			_runOnBuild = run;
			BuildStartTime = DateTime.Now;
			BuildingGame = true;
			CompileBuild(false, OnBuildOutput);
		}

		private void OnAssemblyOutput(object? sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				string parsedMessage = e.Data.Replace($"[{_project.ProjectAssemblyPath}]", string.Empty);

				if (e.Data.Contains("error") && _assemblyCompileErrors.Contains(parsedMessage) == false)
				{
					_assemblyCompileErrors.Add(parsedMessage);
				}
				else if (e.Data.Contains("warning") && _assemblyCompileWarnings.Contains(parsedMessage) == false)
				{
					_assemblyCompileWarnings.Add(parsedMessage);
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
			AssemblyCompileWarnings = _assemblyCompileWarnings;
			AssemblyCompileErrors = _assemblyCompileErrors;
			AssemblyLoaded = true;
			_assemblyCompileErrors = new();
			_assemblyCompileWarnings = new();

			if (AssemblyCompileErrors.Count == 0)
			{
				_project.Scripting.ReadScriptAssembly(_project.AssemblyBinaryPath);
				_project.OpenedScene?.ReloadScripts();

				_project.Logger.LogMessage($"Working clear, no errors found! (Build in " +
					$"{AssemblyBuildEndTime.ToString("HH:mm:ss")}, " +
					$"{Math.Round((AssemblyBuildEndTime -
					AssemblyBuildStartTime).TotalSeconds, 1)} sec)");
			}
			else
			{
				AssemblyCompileErrors.Add($"Assembly load is failed. See following errors... (Failed in " +
					$"{AssemblyBuildEndTime.ToString("HH:mm:ss")}, " +
					$"{Math.Round((AssemblyBuildEndTime -
					AssemblyBuildStartTime).TotalSeconds, 1)} sec)");
			}
		}

		private void OnBuildOutput(object? sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				string parsedMessage = e.Data.Replace($"[{_project.ProjectAssemblyPath}]", string.Empty);

				if (e.Data.Contains("error") && _buildCompileErrors.Contains(parsedMessage) == false)
				{
					_buildCompileErrors.Add(parsedMessage);
				}
				else if (e.Data.Contains("warning") && _buildCompileWarnings.Contains(parsedMessage) == false)
				{
					_buildCompileWarnings.Add(parsedMessage);
				}
			}
			else
			{
				BuildEndTime = DateTime.Now;
				OnBuildCompleted();
			}
		}

		private void OnBuildCompleted()
		{
			BuildCompileErrors = _buildCompileErrors;
			BuildCompileWarnings = _buildCompileWarnings;
			_buildCompileErrors = new();
			_buildCompileWarnings = new();

			Utils.CopyDirectory($@"{_project.ProjectBuildBinaryDirectory}\{_project.Settings.BuildOS}", $@"{_project.Directory}\Build\{_project.Settings.BuildOS}");

			DirectoryInfo directory = new DirectoryInfo($@"{_project.Directory}\Build\{_project.Settings.BuildOS}");
			foreach (FileInfo file in directory.GetFiles())
			{
				if (file.Extension != "" && file.Extension != "dylib" 
					&& file.Extension != ".so" && file.Extension != ".a" && file.Extension != ".dll" 
					&& file.Extension != ".exe" && (file.Extension != ".json" || file.Name.Contains("runtime") == false))
				{
					file.Delete();
				}
			}

			if (Directory.Exists(directory.FullName + @"\Data") == false)
				directory.CreateSubdirectory("Data");

			foreach (FileInfo file in directory.GetFiles())
			{
				if (file.Extension == ".dll" && file.Name != "BEngineCore.dll" && file.Name != $"{_project.Name}.dll")
				{
					file.MoveTo(directory.FullName + @$"\Data\{file.Name}", true);
				}
			}

			Utils.CopyDirectory($@"runtimes\{_project.Settings.BuildOS}\native", $@"{_project.Directory}\Build\{_project.Settings.BuildOS}\Data");

			if (BuildCompileErrors.Count == 0)
			{
				_project.Logger.LogMessage($"Build is completed! (Build in " +
					$"{BuildEndTime.ToString("HH:mm:ss")}, " +
					$"{Math.Round((BuildEndTime -
					BuildStartTime).TotalSeconds, 1)} sec)");
			}
			else
			{
				_project.Logger.LogError($"Build is failed. See following errors... (Failed in" +
					$"{BuildEndTime.ToString("HH:mm:ss")}, " +
					$"{Math.Round((BuildEndTime -
					BuildStartTime).TotalSeconds, 1)} sec)");
			}

			BuildingGame = false;

			if (_runOnBuild && File.Exists($@"{_project.Directory}\Build\{_project.Settings.BuildOS}\{_project.Name}.exe"))
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					WorkingDirectory = $@"{_project.Directory}\Build\{_project.Settings.BuildOS}\",
					FileName = $@"{_project.Directory}\Build\{_project.Settings.BuildOS}\{_project.Name}.exe"
				};

				Process.Start(startInfo);
			}
		}
	}
}
