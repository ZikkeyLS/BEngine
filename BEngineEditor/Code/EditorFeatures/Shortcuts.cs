using BEngine;
using BEngineCore;

namespace BEngineEditor
{
	public class Shortcuts
	{
		private ProjectContext _projectContext;

		private BEngineCore.Input _input => _projectContext.Window.Input;
		private EditorProject _project => _projectContext.CurrentProject;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;

		private DateTime _lastOpenedEditor = DateTime.Now;
		private DateTime _lastSavedScene = DateTime.Now;

		public Shortcuts(ProjectContext context)
		{
			_projectContext = context;
		}

		public void Update()
		{
			if (_projectContext.CurrentProject == null) 
				return;

			if (ControlShiftActive() && _input.IsKeyPressed(Key.Q))
			{
				if ((DateTime.Now - _lastOpenedEditor).TotalSeconds >= 1)
				{
					_lastOpenedEditor = DateTime.Now;
					Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
				}
			}

			if (_compiler.BuildingGame)
				return;

			if (ControlShiftActive() && _input.IsKeyPressed(Key.F))
			{
				_projectContext.SearchingProject = true;
			}

			if (ControlShiftActive() && _input.IsKeyPressed(Key.B))
			{
				_compiler.CompileScripts();
			}

			if (ControlActive() && _input.IsKeyPressed(Key.S) && _project.LoadedScene != null)
			{
				if ((DateTime.Now - _lastSavedScene).TotalSeconds >= 1)
				{
					_lastSavedScene = DateTime.Now;
					_project.LoadedScene.Save<Scene>();
				}			
			}

			if (_compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
			{
				if (ControlShiftActive() && _input.IsKeyPressed(Key.G))
				{
					_compiler.BuildGame();
				}

				if (ControlShiftActive() && _input.IsKeyPressed(Key.R))
				{
					_compiler.BuildGame(true);
				}
			}
		}

		private bool ControlActive()
		{
			return _input.IsKeyPressed(Key.ControlLeft) || _input.IsKeyPressed(Key.ControlRight);
		}

		private bool ShiftActive()
		{
			return _input.IsKeyPressed(Key.ShiftLeft) || _input.IsKeyPressed(Key.ShiftRight);
		}

		private bool ControlShiftActive()
		{
			return ControlActive() && ShiftActive();
		}
	}
}
