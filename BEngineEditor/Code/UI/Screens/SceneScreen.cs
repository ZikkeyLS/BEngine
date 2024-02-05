using ImGuiNET;

namespace BEngineEditor
{
	public class SceneScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _openedScene => _projectContext.CurrentProject.OpenedScene;

		public override void Display()
		{
			ImGui.Begin("Scene");

			ImGui.End();
		}
	}
}
