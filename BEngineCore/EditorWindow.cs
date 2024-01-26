using Dear_ImGui_Sample;
using Eq2k.FileDialog.Example.UI;
using ImGuiNET;
using OpenTK.Windowing.Common;

namespace BEngineCore
{
	public class EditorWindow : Window
	{
		private ImGuiController _controller;
		private PathPicker _projectCreator;
		private PathPicker _projectSelector;

		private string _tempProjectPath = ""; 
		private string _tempProjectName = "NewProject";
		private string _currentProjectPath;

		public EditorWindow(string title = "Window", int x = 1280, int y = 720) : base(title, x, y)
		{

		}

		protected override void OnLoad()
		{
			_controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
			_projectCreator = new PathPicker() { Mode = PathPicker.PickerMode.Folder };
			_projectSelector = new PathPicker() { Mode = PathPicker.PickerMode.File, AllowedFiles = ["*.sln"] };

			_tempProjectPath = Directory.GetCurrentDirectory();
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
			_controller.Update(_window, time);
		}

		protected override void OnPostRender()
		{
			// Enable Docking
			ImGui.DockSpaceOverViewport();
			ImGui.Begin("Project Loader");

			ImGui.InputText("New Project Name", ref _tempProjectName, 128);


			if (ImGui.Button("Select Folder", new System.Numerics.Vector2(100, 25)))
			{
				_projectCreator.ShowModal(Directory.GetCurrentDirectory());
			}

			if (_projectCreator.Render() && !_projectCreator.Cancelled)
			{
				_tempProjectPath = _projectCreator.SelectedFolder;
			}

			ImGui.Text($@"Project will be generated in {_tempProjectPath}\{_tempProjectName}");

			if (ImGui.Button("Create Project", new System.Numerics.Vector2(150, 50))) 
			{
				// Create Project
			}

			ImGui.Separator();

			if (ImGui.Button("Load project", new System.Numerics.Vector2(150, 50)))
			{
				_projectSelector.ShowModal(Directory.GetCurrentDirectory());
			}

			if (_projectSelector.Render() && !_projectSelector.Cancelled)
			{
				// Load Project
			}

			ImGui.End();
			
			_controller.Render();
			ImGuiController.CheckGLError("End of frame");
		}
	}
}
