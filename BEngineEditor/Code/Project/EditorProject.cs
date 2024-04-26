using BEngine;
using BEngineCore;
using System.Diagnostics;

namespace BEngineEditor
{
	public enum ItemTypeSelected : byte
	{
		None = 0,
		Asset,
		Entity
	}

	public class SelectedElement
	{
		public ItemTypeSelected Type { get; set; }
		public object Element { get; set; }

		public SelectedElement()
		{

		}

		public SelectedElement(ItemTypeSelected type, object element)
		{
			Type = type;
			Element = element;
		}
	}

	public class EditorProject : ProjectAbstraction
	{
		private AssetWriter _assetWriter;
		private AssetWatcher _assets;
		private ProjectCompiler _compiler;
		private AssemblyListener _assemblyListener;
		public SelectedElement? SelectedElement;

		public string Name { get; private set; } = string.Empty;
		public string Directory { get; private set; } = string.Empty;
		public ProjectSettings Settings { get; private set; }

		public AssemblyListener AssemblyListener => _assemblyListener;
		public ProjectCompiler Compiler => _compiler;
		public AssetWatcher AssetWatcher => _assets;
		public AssetWriter AssetWriter => _assetWriter;

		public string SolutionPath => $@"{Directory}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Directory}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Directory}\{Name}Build";
		public string ProjectBuildBinaryDirectory => $@"{ProjectBuildDirectory}\bin\Release\net8.0\";
		public string ProjectAssemblyPath => $@"{ProjectAssemblyDirectory}\{Name}Assembly.csproj";
		public string AssemblyBinaryPath =>  $@"{ProjectAssemblyDirectory}\bin\Debug\net8.0\{Name}Assembly.dll";
		public string AssetsDirectory => $@"{ProjectAssemblyDirectory}\Assets";
		public bool EditorAssemblyExists => File.Exists(AssemblyBinaryPath);

		private const int WaitMSIteration = 10;

		public EditorProject(string name, string directory, EngineWindow window) : base(window, true)
		{
			Name = name;
			Directory = directory;
			Settings = new(this);

			ProjectSettings? settings = Settings.Load();
			if (settings != null)
				Settings = settings;

			Settings.UpdateResultPath(this);
		}

		public override void LoadProjectData()
		{
			logger.EnableFileLogs = true;
			logger.ClearFileLogs();

			_compiler = new ProjectCompiler(this);
			_compiler.CompileScripts();

			_assemblyListener = new();
			_assemblyListener.InitializeScriptWatch(this);
			_assemblyListener.OnScriptsChanged += (e) => _compiler.CompileScripts();

			reader = new AssetReader([AssetsDirectory, "./EngineData/Assets"], AssetReaderType.Directory);

			_assetWriter = new AssetWriter(AssetsDirectory, reader);
			_assets = new AssetWatcher(AssetsDirectory, _assetWriter);

			TryLoadLastOpenedScene();
		}

		public override void OnSceneLongLoad(Scene scene)
		{
			Settings.LastOpenedSceneID = scene.GUID;
			LoadSceneOnAssemblyLoaded(scene);
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			LoadedScene?.CallEvent(EventID.EditorStart);
		}

		public void SaveCurrentScene()
		{
			if (Runtime)
			{
				Logger.LogWarning("You can't save scene in runtime mode.");
				return;
			}

			if (LoadedScene == null)
			{
				Logger.LogWarning("You can't save null referenced scene.");
				return;
			}

			LoadedScene.SaveGuaranteed<Scene>(AssetsDirectory + "/" + LoadedScene.SceneName + ".scene");
		}

		public void TryLoadLastOpenedScene()
		{		
			Scene? scene = TryLoadScene(Settings.LastOpenedSceneID);

			if (scene == null)
			{
				Settings.LastOpenedSceneID = string.Empty;
			}
		}

		public async void LoadSceneOnAssemblyLoaded(Scene scene)
		{
			await Task.Run(() =>
			{
				while (_compiler.AssemblyLoaded == false ||
					_compiler.AssemblyCompileErrors.Count > 0 || scripting.ReadyToUse == false)
				{
					Thread.Sleep(WaitMSIteration);
				}

				TryLoadScene(scene, true);
			});
		}

		public async void OpenScriptFile(string filePath)
		{
			await Task.Run(() =>
			{
				switch (Settings.IDE)
				{
					case IDE.VisualStudio:
						RunCMDCommand($"start devenv \"{ProjectAssemblyPath}\"", () =>
						{
							// TO BE Fixed:
							// start devenv doesn't wait any time after run, so we can't know exactly, when VS will be opened
							Thread.Sleep(8000);
							RunCMDCommand($"start devenv /edit \"{filePath}\"");
						});
						break;
					case IDE.VisualStudioCode:
						RunCMDCommand($"code \"./\"", () => 
						{
							RunCMDCommand($"code \"{filePath}\""); 
						});
						break;
				}
			});		
		}

		public void RunCMDCommand(string command, Action? onEnd = null)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.EnableRaisingEvents = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.OutputDataReceived += (a, e) => 
			{
				if (e.Data == null)
				{
					onEnd?.Invoke();
				}
			};
			process.Start();
			process.BeginOutputReadLine();

			process.StandardInput.WriteLine(ProjectAssemblyDirectory[0] + ":");
			process.StandardInput.WriteLine($"cd {ProjectAssemblyDirectory}");
			process.StandardInput.WriteLine(command);

			process.StandardInput.Flush();
			process.StandardInput.Close();
		}
	}
}
