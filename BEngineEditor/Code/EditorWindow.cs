using BEngineCore;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace BEngineEditor
{
	public class EditorWindow : EngineWindow
	{
		private ImGuiController _controller;
		private Shortcuts _shortcuts;

		public ProjectContext ProjectContext { get; private set; }
		public EditorSettings Settings { get; private set; }

		private ProjectLoaderScreen _projectLoader = new();
		private MenuBarScreen _menuBar = new();
		private AssemblyStatusScreen _assemblyStatus = new();
		private ProjectExplorerScreen _projectExplorer = new();

		private const string UIConfigName = "BEngineEditorUI.ini";

		public EditorWindow(string title = "Window", int x = 1280, int y = 720) : base(title, x, y)
		{

		}

		public bool IsKeyPressed(Key key)
		{
			return input.Keyboards[0].IsKeyPressed(key);
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			Scripting.LoadInternalScriptingAPI();

			_controller = new ImGuiController(gl, window, input);
			ImGuiIOPtr io = ImGui.GetIO();
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
			ImGui.LoadIniSettingsFromDisk(UIConfigName);

			ProjectContext = new(this);

			Settings = new EditorSettings();
			EditorSettings? editorSettings = Settings.Load();
			if (editorSettings != null)
				Settings = editorSettings;

			_shortcuts = new Shortcuts(ProjectContext);

			_projectLoader.Initialize(this);
			_menuBar.Initialize(this);
			_assemblyStatus.Initialize(this);
			_projectExplorer.Initialize(this);
		}

		protected override void OnRender(double time)
		{
			_shortcuts.Update();
			_controller.Update((float)time);

			ImGui.DockSpaceOverViewport();

			DisplayUI();

			_controller.Render();

			ProjectContext.CurrentProject?.Logger.InsertSafeLogs();
		}

		protected override void OnUpdate(double time)
		{

		}

		private void DisplayUI()
		{
			_menuBar.Display();

			if (ProjectContext.ProjectLoaded)
				_projectExplorer.Display();

			if (ProjectContext.ProjectLoaded)
				_assemblyStatus.Display();

			if (ProjectContext.SearchingProject)
				_projectLoader.Display();
		}

		protected override void OnClose()
		{
			Settings.Save();
			ProjectContext.CurrentProject?.Settings.Save();

			ImGui.SaveIniSettingsToDisk(UIConfigName);
		}
	}
}
