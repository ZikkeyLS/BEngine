using System.Diagnostics;

namespace BEngineEditor
{
	public class ProjectCompiler
	{
		public  void CompileScriptAssembly(string directory, bool debug = true, EventHandler? onReady = null)
		{
			try
			{
				string mode = debug ? "Debug" : "Release";

				Thread thread = new(() =>
				{
					Process console = new Process();
					console.StartInfo.FileName = "cmd.exe";
					console.EnableRaisingEvents = true;
					console.Exited += onReady;
					console.StartInfo.RedirectStandardInput = true;
					console.StartInfo.RedirectStandardOutput = true;
					console.StartInfo.CreateNoWindow = true;
					console.StartInfo.UseShellExecute = false;
					console.Start();

					console.StandardInput.WriteLine($"dotnet build {directory} -c {mode}");
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

		public void CompileBuild(bool debug = false)
		{

		}

	}
}
