using BEngineCore;
using BEngineEditor.Code;
using ImGuiNET;
using OpenTK.Windowing.Common;
using System.ComponentModel;

namespace BEngineEditor
{
	public class EditorWindow : Window
	{
		private ImGuiController _controller;
		private Shortcuts _shortcuts;

		public ProjectContext ProjectContext { get; private set; }
		public EditorSettings Settings { get; private set; }

		private ProjectLoaderScreen _projectLoader = new();
		private MenuBarScreen _menuBar = new();
		private AssemblyStatusScreen _assemblyStatus = new();
		private ProjectExplorerScreen _projectExplorer = new();

		public EditorWindow(string title = "Window", int x = 1280, int y = 720) : base(title, x, y)
		{

		}

		public bool IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys key)
		{
			return _window.IsKeyDown(key);
		}

		public bool IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys key)
		{
			return _window.IsKeyPressed(key);
		}

		public bool IsKeyUp(OpenTK.Windowing.GraphicsLibraryFramework.Keys key)
		{
			return _window.IsKeyReleased(key);
		}

		protected override void OnLoad()
		{
			_controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);

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

		protected override void MouseWheel(MouseWheelEventArgs obj)
		{
			_controller.MouseScroll(obj.Offset);
		}

		protected override void OnTextInput(TextInputEventArgs obj)
		{
			_controller.PressChar((char)obj.Unicode);
		}

		protected override void OnResize()
		{
			_controller.WindowResized(_window.ClientSize.X, _window.ClientSize.Y);
		}

		protected override void OnPreRender(float time)
		{
			_shortcuts.Update();
			_controller.Update(_window, time);
		}

		protected override void OnPostRender()
		{
			ImGui.DockSpaceOverViewport();

			DisplayUI();

			_controller.Render();
			ImGuiController.CheckGLError("End of frame");

			ProjectContext.CurrentProject?.Logger.InsertSafeLogs();
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

		protected override void OnClose(CancelEventArgs obj)
		{
			Settings.Save();
			ProjectContext.CurrentProject?.Settings.Save();
		}
	}
}
