using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	public class PropertiesScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scripting _scripting => _projectContext.CurrentProject.Scripting;
#pragma warning disable CS8603 // Possible null reference return.
		private SelectedElement _selectedElement => _projectContext.CurrentProject.SelectedElement;
#pragma warning restore CS8603 // Possible null reference return.

		private string? _lastName = string.Empty;

		private bool _showScriptSelection = false;

		public override void Display()
		{
			ImGui.Begin("Properties");

			if (_selectedElement.Type == ItemTypeSelected.Entity)
			{
				SceneEntity selectedEntity = (SceneEntity)_selectedElement.Element;

				if (_projectContext.CurrentProject.OpenedScene.Entities.Contains(selectedEntity) == false)
				{
					_projectContext.CurrentProject.SelectedElement = null;
					return;
				}

				string newName = selectedEntity.Name;

				if (_lastName != selectedEntity.GUID)
				{
					_lastName = selectedEntity.GUID;
					return;
				}

				if (ImGui.InputText("Name", ref newName, 150))
				{
					if (newName == "" || newName == " ")
					{
						newName = "~";
					}

					selectedEntity.Name = newName;
				}

				ImGui.Separator();

				for (int i = 0; i < selectedEntity.Scripts.Count; i++)
				{
					ImGui.BeginGroup();
					ImGui.Text(selectedEntity.Scripts[i].Namespace + "." + selectedEntity.Scripts[i].Name);
					ImGui.EndGroup();
				}

				ImGui.SetCursorPosX((ImGui.GetWindowWidth() - 125) / 4);
				if (ImGui.Button("Add script", new System.Numerics.Vector2(100, 60))) 
				{
					_showScriptSelection = true;
				}

				if (_showScriptSelection)
				{
					if (ImGui.BeginListBox("Select Script"))
					{
						for (int i = 0; i < _scripting.Scripts.Count; i++)
						{
							Scripting.CachedScript currentScript = _scripting.Scripts[i];

							if (selectedEntity.Scripts.Find((script) => script.Namespace == currentScript.Namespace 
								&& script.Name == currentScript.Name) != null)
							{
								continue;
							}

							if (ImGui.Selectable(currentScript.Fullname))
							{
								selectedEntity.AddScript(currentScript);
								_showScriptSelection = false;
							}
						}
					}
					ImGui.EndListBox();
				}
			}

			ImGui.End();
		}
	}
}
