using ImGuiNET;

namespace BEngineEditor
{
	internal class MenuBarScreen : Screen
	{
		private ProjectContext _projectContext;

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;

		}

		public override void Display()
		{
			ImGui.BeginMainMenuBar();

			if (ImGui.Button("Load Project", new System.Numerics.Vector2(100, 50)))
				_projectContext.SearchingProject = true;

			ImGui.EndMainMenuBar();
		}
	}
}
