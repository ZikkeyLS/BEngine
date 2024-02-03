using ImGuiNET;

namespace BEngineEditor
{
	public class PropertiesScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private SelectedElement _selectedElement => _projectContext.CurrentProject.SelectedElement;

		private string? _lastName = string.Empty;

		public override void Display()
		{
			ImGui.Begin("Properties");

			if (_selectedElement.Type == ItemTypeSelected.Entity)
			{
				SceneEntity tempEntity = (SceneEntity)_selectedElement.Element;

				if (_projectContext.CurrentProject.OpenedScene.Entities.Contains(tempEntity) == false)
				{
					_projectContext.CurrentProject.SelectedElement = null;
					return;
				}

				string newName = tempEntity.Name;

				if (_lastName != tempEntity.GUID)
				{
					_lastName = tempEntity.GUID;
					return;
				}

				if (ImGui.InputText("Name", ref newName, 150))
				{
					if (newName == "" || newName == " ")
					{
						newName = "~";
					}

					tempEntity.Name = newName;
				}

				ImGui.Separator();

				ImGui.SetCursorPosX((ImGui.GetWindowWidth() - 125) / 4);
				ImGui.Button("Add script", new System.Numerics.Vector2(100, 60));
			}

			ImGui.End();
		}
	}
}
