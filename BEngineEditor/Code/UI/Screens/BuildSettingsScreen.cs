using BEngineCore;
using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	public class BuildSettingsScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;

		

		protected override void Setup()
		{
			if (additional == null || additional.Length == 0)
				return;

			
		}

		public override void Display()
		{
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
			ImGui.Begin("Build Settings");

			ImGui.Button("Add Scene", new System.Numerics.Vector2(100, 60));
			if (ImGui.BeginPopupContextItem("Add Scene", ImGuiPopupFlags.MouseButtonLeft))
			{
				if (ImGui.BeginListBox("Select Scene"))
				{
					foreach (var pair in _projectContext.CurrentProject.AssetsReader.SceneContext)
					{
						if (ImGui.Selectable(pair.Value.SceneName + $" ({pair.Key})"))
						{
                            var runtimeScenes = _projectContext.CurrentProject.Settings.ProjectRuntimeInfo.RuntimeScenes;
							runtimeScenes.Add((uint)runtimeScenes.Count, new RuntimeScene(pair.Key, pair.Value.SceneName));
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
	}
}
