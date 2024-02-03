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

			for (int i = 0; i < _projectContext.CurrentProject.Scripting.Scripts.Count; i++)
			{
				ImGui.BeginGroup();

				ImGui.Text("Class: " + _projectContext.CurrentProject.Scripting.Scripts[i].Name);
				ImGui.Text("Fields:");
				for (int j = 0; j < _projectContext.CurrentProject.Scripting.Scripts[i].Fields.Count; j++)
					ImGui.Text(_projectContext.CurrentProject.Scripting.Scripts[i].Fields[j].Name);
				ImGui.Text("Methods:");
				for (int j = 0; j < _projectContext.CurrentProject.Scripting.Scripts[i].Methods.Count; j++)
					ImGui.Text(_projectContext.CurrentProject.Scripting.Scripts[i].Methods[j]);

				ImGui.EndGroup();

				ImGui.Text("------------------------------------------------");
			}

			ShowPopups();
			ImGui.End();
		}

		private void ShowPopups()
		{
			if (ImGui.BeginPopupContextWindow("Hierarchy", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
			{
				if (ImGui.Selectable("Create Entity"))
				{
					// add entity with name to list
				}

				ImGui.EndPopup();
			}
		}

		private void RenderEntitiesRecursively()
		{

		}
	}
}
