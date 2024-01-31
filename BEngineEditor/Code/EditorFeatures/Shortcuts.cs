using Silk.NET.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	public class Shortcuts
	{
		private ProjectContext _projectContext;

		protected EditorWindow window => _projectContext.Window;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;

		private DateTime _lastOpenedEditor = DateTime.Now;

		public Shortcuts(ProjectContext context)
		{
			_projectContext = context;
		}

		public void Update()
		{
			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false)
			{
				if ((window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight))
					&& (window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight)) &&
					window.IsKeyPressed(Key.F))
				{
					_projectContext.SearchingProject = true;
				}
			}

			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false)
			{
				if ((window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight))
					&& (window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight)) &&
					window.IsKeyPressed(Key.Q))
				{
					if ((DateTime.Now - _lastOpenedEditor).TotalSeconds >= 1)
					{
						_lastOpenedEditor = DateTime.Now;
						Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
					}
				}
			}

			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false)
			{
				if ((window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight))
					&& (window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight)) &&
					window.IsKeyPressed(Key.B))
				{
					_compiler.CompileScripts();
				}
			}

			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false
				&& _compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
			{
				if ((window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight))
					&& (window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight)) &&
					window.IsKeyPressed(Key.G))
				{
					_compiler.BuildGame();
				}

				if ((window.IsKeyPressed(Key.ControlLeft) || window.IsKeyPressed(Key.ControlRight))
					&& (window.IsKeyPressed(Key.ShiftLeft) || window.IsKeyPressed(Key.ShiftRight)) &&
					window.IsKeyPressed(Key.R))
				{
					_compiler.BuildGame(true);
				}
			}
		}
	}
}
