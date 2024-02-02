using ImGuiNET;

namespace BEngineEditor 
{
	internal class HierarchyScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _currentScene => _projectContext.CurrentProject.OpenedScene;

		public override void Display()
		{
			ImGui.Begin("Hierarchy", ImGuiWindowFlags.HorizontalScrollbar);



			ImGui.End();
		}

		private void RenderEntitiesRecursively()
		{

		}
	}
}
