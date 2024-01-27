using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal class Project
	{
		public string Name { get; private set; }
		public string Path { get; private set; }

		public string SolutionPath => $"{Path}/{Name}.sln";
		public string ProjectAssemblyDirectory => $"{Path}/{Name}Assembly";
		public string ProjectBuildDirectory => $"{Path}/{Name}Build";

		public bool EditorAssemblyExists => File.Exists(ProjectAssemblyDirectory + $"/bin/Debug/net8.0/{Name}Assembly.dll");

		public Project(string name, string path)
		{
			Name = name;
			Path = path;
		}

		public void CompileScriptAssembly(bool debug = true)
		{
			Process console = new Process();
			console.Exited += CompileAssemblyReady;
			console.StartInfo.FileName = @"cmd.exe";
			console.EnableRaisingEvents = true;
			console.Start();

			string mode = debug ? "Debug" : "Release";
			console.StandardInput.WriteLine($"dotnet build -c {mode}");
			console.StandardInput.Flush();
			console.StandardInput.Close();
		}

		private void CompileAssemblyReady(object? sender, EventArgs e)
		{
			Console.WriteLine("Assembly compiled sucessfully!");
		}

		public void CompileBuild(bool debug = false)
		{

		}
	}
}
