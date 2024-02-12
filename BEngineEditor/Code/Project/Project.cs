using BEngineCore;

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

	public class Project
	{
		private Scripting _scripting = new();
		private AssetWorker _assets;
		private ProjectCompiler _compiler;
		private AssemblyListener _assemblyListener;
		private Logger _logger = new();

		public Scene OpenedScene;
		public SelectedElement? SelectedElement;

		public string Name { get; private set; } = string.Empty;
		public string Directory { get; private set; } = string.Empty;
		public ProjectSettings Settings { get; private set; }

		public AssemblyListener AssemblyListener => _assemblyListener;
		public ProjectCompiler Compiler => _compiler;
		public Logger Logger => _logger;
		public AssetWorker AssetWorker => _assets;
		public Scripting Scripting => _scripting;

		public string SolutionPath => $@"{Directory}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Directory}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Directory}\{Name}Build";
		public string ProjectBuildBinaryDirectory => $@"{ProjectBuildDirectory}\bin\Release\net8.0\";
		public string ProjectAssemblyPath => $@"{ProjectAssemblyDirectory}\{Name}Assembly.csproj";
		public string AssemblyBinaryPath =>  $@"{ProjectAssemblyDirectory}\bin\Debug\net8.0\{Name}Assembly.dll";
		public string AssetsDirectory => $@"{ProjectAssemblyDirectory}\Assets";
		public bool EditorAssemblyExists => File.Exists(AssemblyBinaryPath);

		private const int WaitMSIteration = 10;

		public Project(string name, string directory)
		{
			Name = name;
			Directory = directory;
			Settings = new(this);

			ProjectSettings? settings = Settings.Load();
			if (settings != null)
				Settings = settings;

			Settings.UpdateResultPath(this);
		}

		public void LoadProjectData()
		{
			_logger.EnableFileLogs = true;
			_logger.ClearFileLogs();

			_compiler = new ProjectCompiler(this);
			_compiler.CompileScripts();

			_assemblyListener = new();
			_assemblyListener.InitializeScriptWatch(this);
			_assemblyListener.OnScriptsChanged += (e) => _compiler.CompileScripts();

			_assets = new AssetWorker(this);

			TryLoadLastOpenedScene();
		}

		public void TryLoadLastOpenedScene()
		{		
			Scene? scene = TryLoadScene(Settings.LastOpenedSceneID);

			if (scene == null)
			{
				Settings.LastOpenedSceneID = string.Empty;
			}
		}

		public Scene? TryLoadScene(string metaID)
		{
			Scene? scene = AssetData.Load<Scene>(metaID, this);

			if (scene != null)
			{
				OpenedScene?.Save<Scene>();
				Settings.LastOpenedSceneID = scene.GUID;
				LoadSceneOnAssemblyLoaded(scene);
			}

			return scene;
		}

		public async void LoadSceneOnAssemblyLoaded(Scene scene)
		{
			await Task.Run(() =>
			{
				while (_compiler.AssemblyLoaded == false || 
					_compiler.AssemblyCompileErrors.Count > 0 || _scripting.ReadyToUse == false)
					Thread.Sleep(WaitMSIteration);
				OpenedScene = scene;
				OpenedScene.LoadScene();
			});
		}
	}
}
