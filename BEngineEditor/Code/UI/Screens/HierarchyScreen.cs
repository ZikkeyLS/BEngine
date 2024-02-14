using BEngine;
using ImGuiNET;

namespace BEngineEditor
{
	internal class HierarchyScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _scene => _projectContext.CurrentProject.OpenedScene;
		private List<SceneEntity> _entities => _scene.Entities;

		private const string HierarchyPayload = "HierarchyPayload";

		private struct Entry
		{
			public string GUID;
		}

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

			DragAndDrop(current);

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


		private unsafe void DragAndDrop(SceneEntity entity)
		{
			if (ImGui.BeginDragDropSource())
			{
				Entry entry = new Entry() { GUID = entity.GUID };
				ImGui.SetDragDropPayload(HierarchyPayload, (IntPtr)(&entry), (uint)sizeof(Entry));
				ImGui.Text("Entity");
				ImGui.Text($"{entity.Name} ({entity.GUID})");
				ImGui.EndDragDropSource();
			}

			Drop(entity);
		}

		private unsafe void Drop(SceneEntity entity)
		{
			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(HierarchyPayload);

				if (payload.NativePtr != null)
				{
					var entryPointer = (Entry*)payload.Data;
					Entry entry = entryPointer[0];

					SceneEntity? drop = _scene.GetEntity(entry.GUID);

					if (drop != null)
					{
						if (entity.ChildOf(drop) == false)
						{
							entity.Children.Add(drop.GUID);
							drop.Parent = entity.GUID;
						}
					}
				}

				ImGui.EndDragDropTarget();
			}
		}
	}
}
