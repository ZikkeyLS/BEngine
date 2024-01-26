using BEngineCore;
using ImGuiNET;
using OpenTK.Windowing.Common;
using System.Numerics;

namespace BEngineEditor
{
	public class EditorWindow : Window
	{
		private ImGuiController _controller;
		private PathPicker _projectCreator;
		private PathPicker _projectSelector;

		private ProjectContext _projectContext;

		public EditorWindow(string title = "Window", int x = 1280, int y = 720) : base(title, x, y)
		{

		}

		protected override void OnLoad()
		{
			_controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
			_projectCreator = new PathPicker() { Mode = PathPicker.PickerMode.Folder };
			_projectSelector = new PathPicker() { Mode = PathPicker.PickerMode.File, AllowedFiles = ["*.sln"] };

			_projectContext = new ProjectContext();

			_projectContext.TempProjectPath = Directory.GetCurrentDirectory();
			_projectContext.UpdateCreationPathValid(_projectContext.AssembledTempProjectPath);
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

			if (ImGui.InputText("New Project Name", ref _projectContext.TempProjectName, 128))
			{
				_projectContext.UpdateCreationPathValid(_projectContext.AssembledTempProjectPath);
			}

			if (ImGui.Button("Select Folder", new Vector2(100, 25)))
			{
				_projectCreator.ShowModal(Directory.GetCurrentDirectory());
			}

			if (_projectCreator.Render() && !_projectCreator.Cancelled)
			{
				_projectContext.TempProjectPath = _projectCreator.SelectedFolder;
				_projectContext.UpdateCreationPathValid(_projectContext.AssembledTempProjectPath);
			}

			ImGui.Text("Project will be generated in " + _projectContext.AssembledTempProjectPath);

			if (_projectContext.ValidTempProjectPath)
			{
				if (ImGui.Button("Create Project", new Vector2(150, 50)))
				{
					_projectContext.CreateProject();
				}
			}
			else
			{
				ImGui.TextColored(new Vector4(1, 0, 0, 1), "Invalid path: This directory already exists or name is empty!");
			}

			ImGui.Separator();

			if (ImGui.Button("Load project", new Vector2(150, 50)))
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
