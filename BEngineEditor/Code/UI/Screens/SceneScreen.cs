using BEngineCore;
using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	public class SceneScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _openedScene => _projectContext.CurrentProject.LoadedScene;

		private FrameBuffer _frameBuffer;

		protected override void Setup()
		{
			_frameBuffer = (FrameBuffer)additional[0];
		}

		public override void Display()
		{
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
			ImGui.Begin("Scene");

			Vector2 size = ImGui.GetContentRegionAvail();

			_frameBuffer.RescaleFrameBuffer((uint)size.X, (uint)size.Y);

			ImGui.Image((nint)_frameBuffer.GetFrameTexture(), ImGui.GetContentRegionAvail(), Vector2.UnitY, Vector2.UnitX);

			bool focused =
				ImGui.IsWindowFocused() ||
				(ImGui.IsWindowHovered() && _projectContext.Window.Input.IsButtonPressed(BEngine.MouseButton.Middle));

			_projectContext.CurrentProject.Graphics.EnableNativeCameraMovement = focused;
			if (focused)
				ImGui.SetWindowFocus();

			ImGui.End();
			ImGui.PopStyleVar();
		}
	}
}
