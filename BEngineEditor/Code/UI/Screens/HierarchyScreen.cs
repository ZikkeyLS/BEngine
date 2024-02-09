using ImGuiNET;

namespace BEngineEditor
{
	internal class HierarchyScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _scene => _projectContext.CurrentProject.OpenedScene;
		private List<SceneEntity> _entities => _scene.Entities;

		public override void Display()
		{
			ImGui.Begin("Hierarchy", ImGuiWindowFlags.HorizontalScrollbar);

			bool open = ImGui.TreeNodeEx(_scene.SceneName, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Leaf);

			if (open)
			{
				for (int i = 0; i < _entities.Count; i++)
				{
					if (_entities[i].Parent == null)
					{
						ShowEntitiesRecursively(_entities[i], _entities[i]);
					}

				}

				ImGui.TreePop();
			}

			ShowPopups();
			ImGui.End();
		}

		private void ShowEntitiesRecursively(SceneEntity current, SceneEntity root)
		{
			ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;

			if (current == null)
				return;

			if (current.Children.Count == 0)
				flags |= ImGuiTreeNodeFlags.Leaf;

			ImGui.PushID(current.GUID);
			bool open = ImGui.TreeNodeEx(current.Name, flags);

			if (ImGui.IsItemClicked())
			{
				_projectContext.CurrentProject.SelectedElement = new SelectedElement(ItemTypeSelected.Entity, current);
			}

			//if (directory != _rootAssetsDirectory && directory != _rootAssetsDirectory)
			//	DragAndDrop(directory, new DirectoryInfo(directory).Name, false);
			//else
			//	Drop(directory, false);

			if (ImGui.BeginPopupContextItem(current.GUID, ImGuiPopupFlags.MouseButtonRight))
			{
				if (ImGui.Selectable("Create Entity"))
				{
					current.Children.Add(_scene.CreateEntity("New Entity", current).GUID);
				}

				if (ImGui.Selectable("Delete"))
				{
					_scene.RemoveEntity(current);
				}
				ImGui.EndPopup();
			}
			ImGui.PopID();

			if (open)
			{
				for (int i = 0; i < current.Children.Count; i++)
				{
					ShowEntitiesRecursively(_entities.Find((entity) => entity.GUID == current.Children[i]), root);
				}

				ImGui.TreePop();
			}
		}

		private void ShowPopups()
		{
			if (ImGui.BeginPopupContextWindow("Hierarchy", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
			{
				if (ImGui.Selectable("Create Entity"))
				{
					_scene.CreateEntity("New Entity");
				}

				ImGui.EndPopup();
			}
		}
	}
}
