using BEngine;
using BEngineCore;
using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace BEngineEditor
{
	public class EditorWindow : EngineWindow
	{
		public enum FocusControlledBy : byte
		{
			None = 0,
			Scene = 1,
			Game = 2
		}

		private ImGuiController _controller;
		private Shortcuts _shortcuts;

		public ProjectContext ProjectContext { get; private set; }
		public EditorSettings Settings { get; private set; }
		public FocusControlledBy FocusedBy { get; set; } = FocusControlledBy.None;

		private ProjectLoaderScreen _projectLoader = new();
		private MenuBarScreen _menuBar = new();
		private ConsoleScreen _assemblyStatus = new();
		private ProjectExplorerScreen _projectExplorer = new();
		private SceneScreen _scene = new();
		private GameScreen _game = new();
		private HierarchyScreen _hierarchy = new();
		private PropertiesScreen _properties = new();
		private BuildSettingsScreen _buildSettings = new();

		private FrameBuffer _sceneBuffer;
		private FrameBuffer _gameBuffer;

		private const string UIConfigName = "BEngineEditorUI.ini";

		public EditorWindow(string title = "Window", int x = 1280, int y = 720) : base(title, x, y)
		{

		}

		protected override void OnLoad()
		{
			base.OnLoad();

			Scripting.LoadInternalScriptingAPI();

			ImGuiFontConfig fontConfig = new ImGuiFontConfig(@"Fonts\ArialRegular.ttf", 16, (a) => a.Fonts.GetGlyphRangesCyrillic());
			_controller = new ImGuiController(gl, window, inputContext, fontConfig);
			ImGuiIOPtr io = ImGui.GetIO();
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
			ImGui.LoadIniSettingsFromDisk(UIConfigName);

			ImGui.StyleColorsClassic();

			ProjectContext = new(this);

			Settings = new EditorSettings();
			EditorSettings? editorSettings = Settings.Load();
			if (editorSettings != null)
				Settings = editorSettings;

			_shortcuts = new Shortcuts(ProjectContext);

			_sceneBuffer = graphics.CreateBuffer("Scene", (uint)window.Size.X, (uint)window.Size.Y, true);
			_gameBuffer = graphics.CreateBuffer("Game", (uint)window.Size.X, (uint)window.Size.Y, false);

			_projectLoader.Initialize(this);
			_menuBar.Initialize(this);
			_assemblyStatus.Initialize(this);
			_projectExplorer.Initialize(this);
			_scene.Initialize(this, _sceneBuffer);
			_game.Initialize(this, _gameBuffer);
			_hierarchy.Initialize(this);
			_properties.Initialize(this);
			_buildSettings.Initialize(this);
		}

		protected override void OnRender(double time)
		{
			_controller.Update((float)time);

			graphics.Render((float)time);

			ImGui.DockSpaceOverViewport();
			DisplayUI();
			_controller.Render();

			ProjectContext.CurrentProject?.Logger.InsertSafeLogs();
		}

		protected override void OnUpdate(double time)
		{
			if (ProjectContext.CurrentProject != null)
			{
				ProjectContext.CurrentProject.Time.RawDeltaTime = (float)time;
				ProjectContext.CurrentProject.LoadedScene?.CallEvent(EventID.EditorUpdate, ProjectContext.CurrentProject.Scripting.Scripts);
				if (ProjectContext.CurrentProject.Runtime && ProjectContext.CurrentProject.Pause == false)
				{
					ProjectContext.CurrentProject.LoadedScene?.CallEvent(EventID.Update);
				}
			}

			_shortcuts.Update();
			input.Clean();
		}

		private void DisplayUI()
		{
			_menuBar.Display();

			if (ProjectContext.ProjectLoaded && Settings.BuildSettingsOpened)
				_buildSettings.Display();

			if (ProjectContext.SearchingProject)
				_projectLoader.Display();

			if (ProjectContext.ProjectLoaded)
				_projectExplorer.Display();

			if (ProjectContext.ProjectLoaded)
				_assemblyStatus.Display();

			if (ProjectContext.ProjectLoaded && ProjectContext.CurrentProject.LoadedScene != null)
			{
				_scene.Display();
				_game.Display();
				_hierarchy.Display();
				if (ProjectContext.CurrentProject.SelectedElement != null 
					&& ProjectContext.CurrentProject.SelectedElement.Type != ItemTypeSelected.None)
				{
					_properties.Display();
				}
			}
		}

		protected override void OnClose()
		{
			Settings.Save();
			ProjectContext.CurrentProject?.Settings.Save();
			if (ProjectContext.CurrentProject?.LoadedScene != null)
				ProjectContext.CurrentProject?.SaveCurrentScene();

			ImGui.SaveIniSettingsToDisk(UIConfigName);
		}
	}
}
