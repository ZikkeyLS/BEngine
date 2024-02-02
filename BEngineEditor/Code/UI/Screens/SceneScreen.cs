using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	public class SceneScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scene _openedScene => _projectContext.CurrentProject.OpenedScene;

		public override void Display()
		{
			ImGui.Begin(_openedScene.SceneName);

			ImGui.End();
		}
	}
}
