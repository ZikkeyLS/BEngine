using BEngineCore;
using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	public class GameScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;

		private FrameBuffer _frameBuffer;

		protected override void Setup()
		{
			if (additional == null || additional.Length == 0)
				return;

			_frameBuffer = (FrameBuffer)additional[0];
		}

		public override void Display()
		{
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
			ImGui.Begin("Game", ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.AlwaysAutoResize);

			ImGui.BeginMenuBar();

			float runtimeStateButtonWidth = 100f;
			float spacing = 75f;
			ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - runtimeStateButtonWidth / 2 - spacing);
			if (ImGui.Button(_projectContext.CurrentProject.Runtime ? "Stop" : "Start", new Vector2(runtimeStateButtonWidth, 25)))
			{
				if(!_projectContext.CurrentProject.Runtime)
				{
					_projectContext.CurrentProject.SaveCurrentScene();
				}
				_projectContext.CurrentProject.SwipeRuntime();
			}

			ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - runtimeStateButtonWidth / 2 + spacing);
			if (ImGui.Button(_projectContext.CurrentProject.Pause ? "Unpause" : "Pause", new Vector2(runtimeStateButtonWidth, 25)))
			{
				_projectContext.CurrentProject.SwipePause();
			}

			ImGui.EndMenuBar();
	
			Vector2 size = ImGui.GetContentRegionAvail();
			_frameBuffer.RescaleFrameBuffer((uint)size.X, (uint)size.Y);

			ImGui.Image((nint)_frameBuffer.GetFrameTexture(), ImGui.GetContentRegionAvail(), Vector2.UnitY, Vector2.UnitX);

			bool focused = ImGui.IsWindowFocused();
			bool setFocused = ImGui.IsWindowHovered()
				&& _projectContext.CurrentProject.Input.IsButtonPressed(BEngine.MouseButton.Middle);
			if (setFocused)
			{
				if (window.FocusedBy == EditorWindow.FocusControlledBy.None)
				{
					if (!focused)
					{
						ImGui.SetWindowFocus();
					}
					window.FocusedBy = EditorWindow.FocusControlledBy.Game;
				}
			}
			else if (window.FocusedBy == EditorWindow.FocusControlledBy.Game && !_projectContext.CurrentProject.Input.IsButtonPressed(BEngine.MouseButton.Middle))
			{
				window.FocusedBy = EditorWindow.FocusControlledBy.None;
			}

			ImGui.End();
			ImGui.PopStyleVar();
		}
	}
}
