using Silk.NET.Input;

namespace BEngineEditor
{
	public class Shortcuts
	{
		private ProjectContext _projectContext;

		protected EditorWindow window => _projectContext.Window;
		private Project _project => _projectContext.CurrentProject;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;

		private DateTime _lastOpenedEditor = DateTime.Now;
		private DateTime _lastSavedScene = DateTime.Now;

		public Shortcuts(ProjectContext context)
		{
			_projectContext = context;
		}

		public void Update()
		{
			if (_projectContext == null) 
				return;

			if (ControlShiftActive() && window.IsKeyPressed(Key.Q))
			{
				if ((DateTime.Now - _lastOpenedEditor).TotalSeconds >= 1)
				{
					_lastOpenedEditor = DateTime.Now;
					Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
				}
			}

			if (_compiler.BuildingGame)
				return;

			if (ControlShiftActive() && window.IsKeyPressed(Key.F))
			{
				_projectContext.SearchingProject = true;
			}

			if (ControlShiftActive() && window.IsKeyPressed(Key.B))
			{
				_compiler.CompileScripts();
			}

			if (ControlActive() && window.IsKeyPressed(Key.S) && _project.OpenedScene != null)
			{
				if ((DateTime.Now - _lastSavedScene).TotalSeconds >= 1)
				{
					_lastSavedScene = DateTime.Now;
					_project.OpenedScene.Save<Scene>();
				}			
			}

			if (_compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
			{
				if (ControlShiftActive() && window.IsKeyPressed(Key.G))
				{
					_compiler.BuildGame();
				}

				if (ControlShiftActive() && window.IsKeyPressed(Key.R))
				{
					_compiler.BuildGame(true);
				}
			}
		}

		private bool ControlActive()
		{
			return window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight);
		}

		private bool ShiftActive()
		{
			return window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight);
		}

		private bool ControlShiftActive()
		{
			return ControlActive() && ShiftActive();
		}
	}
}
