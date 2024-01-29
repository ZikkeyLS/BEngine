using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BEngineEditor
{
	internal class MenuBarScreen : Screen
	{
		private ProjectContext _projectContext;

		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;
		private ProjectSettings _settings => _projectContext.CurrentProject.Settings;

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;

		}

		public override void Display()
		{
			ImGui.BeginMainMenuBar();

			if (_projectContext.CurrentProject != null && ImGui.BeginMenu("Actions"))
			{
				if (ImGui.MenuItem("Load Project", "Ctrl+Shift+F"))
				{
					_projectContext.SearchingProject = true;
				}

				if (ImGui.MenuItem("Open Code Editor", "Ctrl+Shift+Q"))
				{
					Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
				}

				if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false)
				{
					if (ImGui.MenuItem("Reload assembly", "Ctrl+Shift+B"))
					{
						_compiler.CompileScripts();
					}
				}

				if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false
					&& _compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
				{
					if (ImGui.MenuItem("Build", "Ctrl+Shift+G"))
					{
						_compiler.BuildGame();
					}

					if (ImGui.MenuItem("Build and Run", "Ctrl+Shift+R"))
					{
						_compiler.BuildGame(true);
					}
				}

				ImGui.EndMenu();
			}

			if (_projectContext.CurrentProject != null && ImGui.BeginMenu("BuildOS"))
			{
				if (ImGui.MenuItem(ProjectCompiler.Win64, "", _compiler.IsCurrentOS(ProjectCompiler.Win64))) 
				{ 
					_settings.BuildOS = ProjectCompiler.Win64;
				}
				if (ImGui.MenuItem(ProjectCompiler.Win86, "", _compiler.IsCurrentOS(ProjectCompiler.Win86))) 
				{ 
					_settings.BuildOS = ProjectCompiler.Win86;
				}
				if (ImGui.MenuItem("Linux", "", _compiler.IsCurrentOS(ProjectCompiler.Linux64))) 
				{ 
					_settings.BuildOS = ProjectCompiler.Linux64;
				}
				if (ImGui.MenuItem("MacOS", "", _compiler.IsCurrentOS(ProjectCompiler.Osx64))) 
				{ 
					_settings.BuildOS = ProjectCompiler.Osx64;
				}

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}
	}
}
