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

			if (ImGui.Button("Load Project"))
				_projectContext.SearchingProject = true;

			ImGui.EndMainMenuBar();
		}
	}
}
