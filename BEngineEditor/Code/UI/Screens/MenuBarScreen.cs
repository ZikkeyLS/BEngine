using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	internal class MenuBarScreen : Screen
	{
		private ProjectContext _projectContext;

		private EditorProject _project => _projectContext.CurrentProject;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;
		private ProjectSettings _settings => _projectContext.CurrentProject.Settings;

		private Texture engineIcon;

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
			engineIcon = new Texture(EditorGlobals.IconPath, Graphics.gl);	
		}

		public override void Display()
		{
			ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(0, 15));
			ImGui.BeginMainMenuBar();

			ImGui.SetCursorPosY(2);
			ImGui.Image((nint)engineIcon.ID, new System.Numerics.Vector2(40, 40));

			if (_projectContext.CurrentProject == null)
			{
				ImGui.EndMainMenuBar();
				ImGui.PopStyleVar();
				return;
			}

			if (ImGui.BeginMenu("Actions"))
			{
				if (ImGui.MenuItem("Open Code Editor", "Ctrl+Shift+Q"))
				{
					Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
				}

				if (_compiler.BuildingGame)
					return;

				if (ImGui.MenuItem("Load Project", "Ctrl+Shift+F"))
				{
					_projectContext.SearchingProject = true;
				}

				if (ImGui.MenuItem("Reload assembly", "Ctrl+Shift+B"))
				{
					_compiler.CompileScripts();
				}

				if (ImGui.MenuItem("Save Scene", "Ctrl+S") && _project.LoadedScene != null)
				{
					_project.LoadedScene.SaveGuaranteed<Scene>(_project.AssetsDirectory + "/" + _project.LoadedScene.SceneName + ".scene");
				}

				if (_compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
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

			if (ImGui.BeginMenu("BuildOS"))
			{
				if (ImGui.MenuItem("Windows", "", _compiler.IsCurrentOS(ProjectCompiler.Win64))) 
				{ 
					_settings.BuildOS = ProjectCompiler.Win64;
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

			if (ImGui.BeginMenu("IDE"))
			{
				if (ImGui.MenuItem("Visual Studio", "", _settings.IDE == IDE.VisualStudio))
				{
					_settings.IDE = IDE.VisualStudio;
				}
				if (ImGui.MenuItem("Visual Studio Code", "", _settings.IDE == IDE.VisualStudioCode))
				{
					_settings.IDE = IDE.VisualStudioCode;
				}

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
			ImGui.PopStyleVar();
		}
	}
}
