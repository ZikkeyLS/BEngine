using BEngineCore;
using ImGuiNET;
using System.Globalization;

namespace BEngineEditor
{
	internal class HierarchyScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _scene => _projectContext.CurrentProject.LoadedScene;
		private List<SceneEntity> _entities => _scene.Entities;

		private const string HierarchyPayload = "HierarchyPayload";

		private System.Numerics.Vector4 currentColor;

		private const int PaddingY = 5;

		private bool _dragging = false;

		public unsafe override void Display()
		{
			ImGui.Begin("Hierarchy", ImGuiWindowFlags.HorizontalScrollbar);
			currentColor = *ImGui.GetStyleColorVec4(ImGuiCol.Header);

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

				if (_entities.Count != 0 && _dragging)
				{
					ImGui.ColorButton("DropArea" + _entities.Count, currentColor,
						ImGuiColorEditFlags.NoTooltip |
						ImGuiColorEditFlags.NoPicker |
						ImGuiColorEditFlags.NoDragDrop,
						new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, PaddingY));
					DropPosition(null);
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

			if (_dragging)
			{
				ImGui.ColorButton("DropArea" + (current.GUID), currentColor,
					ImGuiColorEditFlags.NoTooltip |
					ImGuiColorEditFlags.NoPicker |
					ImGuiColorEditFlags.NoDragDrop,
					new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, PaddingY));
				DropPosition(current);
			}

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
					current.AddChild(_scene.CreateEntity("New Entity", current));
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
				if (current.Children.Count == 0)
				{
					ImGui.TreePop();
					return;
				}

				List<SceneEntity> sorted = current.Children.ToList();
				SortChildren(sorted);

				for (int i = 0; i < sorted.Count; i++)
				{
					ShowEntitiesRecursively(sorted[i], root);
				}

				if (_dragging)
				{
					ImGui.ColorButton("DropArea" + (sorted.Count), currentColor,
						ImGuiColorEditFlags.NoTooltip |
						ImGuiColorEditFlags.NoPicker |
						ImGuiColorEditFlags.NoDragDrop,
						new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, PaddingY));
					DropPosition(null);
				}

				ImGui.TreePop();
			}
		}

		private void SortChildren(List<SceneEntity> entities)
		{
			SceneEntity temp;
			for (int write = 0; write < entities.Count; write++)
			{
				for (int sort = 0; sort < entities.Count - 1; sort++)
				{
					if (_entities.IndexOf(entities[sort]) > _entities.IndexOf(entities[sort + 1]))
					{
						temp = entities[sort + 1];
						entities[sort + 1] = entities[sort];
						entities[sort] = temp;
					}
				}
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
				_dragging = true;
				ImGui.SetDragDropPayload(HierarchyPayload, (IntPtr)(&entity), (uint)sizeof(SceneEntity));
				ImGui.Text("Entity");
				ImGui.Text($"{entity.Name} ({entity.GUID})");
				ImGui.EndDragDropSource();
			}

			ImGuiPayloadPtr payload = ImGui.GetDragDropPayload();
			if ((payload.NativePtr == null && _dragging) || 
				(payload.NativePtr != null && payload.IsDataType(HierarchyPayload) == false && _dragging))
			{
				_dragging = false;
			}

			Drop(entity);
		}

		private unsafe void DropPosition(SceneEntity entity)
		{
			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(HierarchyPayload);

				if (payload.NativePtr != null)
				{
					_dragging = false;
					var entryPointer = (SceneEntity*)payload.Data;
					SceneEntity entry = entryPointer[0];

					if (entity != null && entity != entry)
					{
						int entryIndex = _entities.IndexOf(entry);
						int entityIndex = _entities.IndexOf(entity);

						if (entity.Parent != null)
						{
							if (entity.Parent != entry.Parent)
								entry.SetParent(entity.Parent);
						}
						else if (entry.Parent != null)
						{
							entry.ClearParent();
						}

						_entities.Insert(entityIndex, entry);
						if (entryIndex >= entityIndex)
							entryIndex += 1;
						_entities.RemoveAt(entryIndex);
					}
					else if (entity == null)
					{
						int entryIndex = _entities.IndexOf(entry);

						_entities.RemoveAt(entryIndex);
						_entities.Add(entry);
					}
				}

				ImGui.EndDragDropTarget();
			}
		}

		private unsafe void Drop(SceneEntity entity)
		{
			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(HierarchyPayload);

				if (payload.NativePtr != null)
				{
					_dragging = false;
					var entryPointer = (SceneEntity*)payload.Data;
					SceneEntity entry = entryPointer[0];

					if (entry != null)
					{
						entry.SetParent(entity);
					}
				}

				ImGui.EndDragDropTarget();
			}
		}
	}
}
