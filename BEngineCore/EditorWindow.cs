using Dear_ImGui_Sample;
using Eq2k.FileDialog.Example.UI;
using ImGuiNET;

namespace BEngineCore
{
	public class EditorWindow : Window
	{
		private ImGuiController _controller;
		private PathPicker _projectPicker;

		protected override void OnLoad()
		{
			_controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
			_projectPicker = new PathPicker();
			_projectPicker.Mode = PathPicker.PickerMode.Folder;
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
			ImGui.ShowDemoWindow();

			if (_projectPicker.Render() && !_projectPicker.Cancelled)
			{
				// Load another project
			}

			if (ImGui.Button("Folder", new System.Numerics.Vector2(100, 30)))
			{
				_projectPicker.ShowModal(Directory.GetCurrentDirectory());
			}
			
			_controller.Render();
			ImGuiController.CheckGLError("End of frame");
		}
	}
}
