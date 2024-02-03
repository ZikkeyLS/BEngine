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

			for (int i = 0; i < _currentScene.Entities.Count; i++)
			{
				if (_currentScene.Entities[i].Parent == null)
					ShowEntitiesRecursively(_currentScene.Entities[i], _currentScene.Entities[i]);
			}

			ShowPopups();
			ImGui.End();
		}

		private void ShowEntitiesRecursively(SceneEntity current, SceneEntity root)
		{
			ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;

			if (current.Chilren.Count == 0)
				flags |= ImGuiTreeNodeFlags.Leaf;

			bool open = ImGui.TreeNodeEx(current.Name, flags);

			if (ImGui.IsItemClicked())
			{
				//if (directory == root)
				//{
				//	_currentDirectoryOpened = string.Empty;
				//}
				//else
				//{
				//	_currentDirectoryOpened = directory.Replace(root + @"\", string.Empty);
				//}
			}

			//if (directory != _rootAssetsDirectory && directory != _rootAssetsDirectory)
			//	DragAndDrop(directory, new DirectoryInfo(directory).Name, false);
			//else
			//	Drop(directory, false);

			if (open)
			{
				for (int i = 0; i < current.Chilren.Count; i++)
				{
					ShowEntitiesRecursively(current.Chilren[i], root);
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
					// add entity with name to list
					_currentScene.Entities.Add(new SceneEntity("TestEntity"));
				}

				ImGui.EndPopup();
			}
		}

		private void RenderEntitiesRecursively()
		{

		}
	}
}
