using BEngineCore;
using static System.Formats.Asn1.AsnWriter;

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
		private AssetWorker _assets;
		private ProjectCompiler _compiler;
		private AssemblyListener _assemblyListener;

		public Scene OpenedScene;
		public SelectedElement? SelectedElement;

		public string Name { get; private set; } = string.Empty;
		public string Directory { get; private set; } = string.Empty;
		public ProjectSettings Settings { get; private set; }

		public AssemblyListener AssemblyListener => _assemblyListener;
		public ProjectCompiler Compiler => _compiler;
		public AssetWorker AssetWorker => _assets;

		public string SolutionPath => $@"{Directory}\{Name}.sln";
		public string ProjectAssemblyDirectory => $@"{Directory}\{Name}Assembly";
		public string ProjectBuildDirectory => $@"{Directory}\{Name}Build";
		public string ProjectBuildBinaryDirectory => $@"{ProjectBuildDirectory}\bin\Release\net8.0\";
		public string ProjectAssemblyPath => $@"{ProjectAssemblyDirectory}\{Name}Assembly.csproj";
		public string AssemblyBinaryPath =>  $@"{ProjectAssemblyDirectory}\bin\Debug\net8.0\{Name}Assembly.dll";
		public string AssetsDirectory => $@"{ProjectAssemblyDirectory}\Assets";
		public bool EditorAssemblyExists => File.Exists(AssemblyBinaryPath);

		private const int WaitMSIteration = 10;

		public EditorProject(string name, string directory)
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

			reader = new AssetReader(AssetsDirectory, AssetReaderType.Directory);
			_assets = new AssetWorker(this, reader);

			TryLoadLastOpenedScene();
		}

		public override void OnScenePreLoaded(Scene scene)
		{
			Settings.LastOpenedSceneID = scene.GUID;
			LoadSceneOnAssemblyLoaded(scene);
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
					Thread.Sleep(WaitMSIteration);
				OpenedScene = scene;
				OpenedScene.LoadScene();
			});
		}
	}
}
