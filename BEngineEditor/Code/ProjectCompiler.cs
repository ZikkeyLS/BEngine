using System.Diagnostics;

namespace BEngineEditor
{
	public class ProjectCompiler
	{
		public bool Compiling { get; private set; } = false;
		public bool CompileAfterwards { get; private set; } = false;

		Process _assemblyCompilation;

		public void CompileScriptAssembly(string directory, bool debug = true, DataReceivedEventHandler? onOutput = null)
		{
			string mode = debug ? "Debug" : "Release";

			if (_assemblyCompilation != null)
				_assemblyCompilation.Close();

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

			_assemblyCompilation.StandardInput.WriteLine($"dotnet build {directory} -c {mode}");
			_assemblyCompilation.StandardInput.Flush();
			_assemblyCompilation.StandardInput.Close();
		}

		public void CompileBuild(bool debug = false)
		{

		}

	}
}
