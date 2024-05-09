using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	public class BuildSettingsScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private bool _dragging = false;

		private const int PaddingY = 5;
		private const string BuildSettingsPayload = "BuildSettingsPayload";


		protected override void Setup()
		{
			if (additional == null || additional.Length == 0)
				return;
		}

		public override void Display()
		{
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0, 0));
			ImGui.Begin("Build Settings", ImGuiWindowFlags.HorizontalScrollbar);

			var runtimeScenes = _projectContext.CurrentProject.Settings.ProjectRuntimeInfo.RuntimeScenes;

			for (int i = 0; i < runtimeScenes.Count; i++)
			{
				var runtime = runtimeScenes[i];

				ImGui.PushID(runtime.Name);

				ImGui.Dummy(new System.Numerics.Vector2(0, PaddingY));
				ImGui.Dummy(new System.Numerics.Vector2(0, 0));
				ImGui.SameLine();

				ImGui.BeginGroup();

				ImGui.Button($"{runtime.Name} ({runtime.GUID})");
				Drag(new KeyValuePair<int, RuntimeScene>(i, runtime));
				Drop(new KeyValuePair<int, RuntimeScene>(i, runtime));

				ImGui.EndGroup();

				if (ImGui.BeginPopupContextItem(runtime.GUID, ImGuiPopupFlags.MouseButtonRight))
				{
					if (ImGui.Selectable("Remove"))
					{
						runtimeScenes.RemoveAt(i);
					}

					ImGui.EndPopup();
				}

				ImGui.PopID();
			}

			ImGui.Button("Add Scene", new System.Numerics.Vector2(100, 60));
			if (ImGui.BeginPopupContextItem("Add Scene", ImGuiPopupFlags.MouseButtonLeft))
			{
				if (ImGui.BeginListBox("Select Scene"))
				{
					foreach (var pair in _projectContext.CurrentProject.AssetsReader.SceneContext)
					{
						if (FindSceneByGUID(pair.Value.GUID) == null && ImGui.Selectable(pair.Value.SceneName + $" ({pair.Key})"))
						{
							runtimeScenes.Add(new RuntimeScene(pair.Key, pair.Value.SceneName));
							ImGui.CloseCurrentPopup();
						}
					}
				}
				ImGui.EndListBox();
				ImGui.EndPopup();
			}

			ImGui.End();
			ImGui.PopStyleVar();
		}

		private RuntimeScene? FindSceneByGUID(string guid)
		{
			var runtimeScenes = _projectContext.CurrentProject.Settings.ProjectRuntimeInfo.RuntimeScenes;

			foreach (var runtimeScene in runtimeScenes)
			{
				if (runtimeScene.GUID == guid)
				{
					return runtimeScene;
				}
			}

			return null;
		}

		private unsafe void Drag(KeyValuePair<int, RuntimeScene> scene)
		{
			if (ImGui.BeginDragDropSource())
			{
				_dragging = true;
				ImGui.SetDragDropPayload(BuildSettingsPayload, (IntPtr)(&scene), (uint)sizeof(KeyValuePair<int, RuntimeScene>));
				ImGui.Text("Scene");
				ImGui.Text($"{scene.Value.Name} ({scene.Value.GUID})");
				ImGui.EndDragDropSource();
			}

			ImGuiPayloadPtr payload = ImGui.GetDragDropPayload();
			if ((payload.NativePtr == null && _dragging) ||
				(payload.NativePtr != null && payload.IsDataType(BuildSettingsPayload) == false && _dragging))
			{
				_dragging = false;
			}
		}

		private unsafe void Drop(KeyValuePair<int, RuntimeScene> scene)
		{
			var runtimeScenes = _projectContext.CurrentProject.Settings.ProjectRuntimeInfo.RuntimeScenes;

			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(BuildSettingsPayload);

				if (payload.NativePtr != null)
				{
					_dragging = false;
					var entryPointer = (KeyValuePair<int, RuntimeScene>*)payload.Data;
					KeyValuePair<int, RuntimeScene> entry = entryPointer[0];

					RuntimeScene oldRuntimeScene = scene.Value;

					runtimeScenes[scene.Key] = entry.Value;
					runtimeScenes[entry.Key] = oldRuntimeScene;
				}

				ImGui.EndDragDropTarget();
			}
		}
	}
}
