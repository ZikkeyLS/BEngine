using BEngine;
using BEngineCore;
using ImGuiNET;
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
		private SceneScreen _scene = new();
		private HierarchyScreen _hierarchy = new();
		private PropertiesScreen _properties = new();

		private FrameBuffer _sceneBuffer;

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

			_sceneBuffer = graphics.CreateBuffer("Scene", (uint)window.Size.X, (uint)window.Size.Y);

			_projectLoader.Initialize(this);
			_menuBar.Initialize(this);
			_assemblyStatus.Initialize(this);
			_projectExplorer.Initialize(this);
			_scene.Initialize(this, _sceneBuffer);
			_hierarchy.Initialize(this);
			_properties.Initialize(this);
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
				ProjectContext.CurrentProject.LoadedScene?.CallEvent(EventID.EditorUpdate);
			}

			_shortcuts.Update();
			input.Clean();
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

			if (ProjectContext.ProjectLoaded && ProjectContext.CurrentProject.LoadedScene != null)
			{
				_scene.Display();
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
			ProjectContext.CurrentProject?.LoadedScene?.Save<Scene>();

			ImGui.SaveIniSettingsToDisk(UIConfigName);
		}
	}
}
