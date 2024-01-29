using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor.Code
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
				if ((window.IsKeyDown(Keys.LeftControl) || window.IsKeyDown(Keys.RightControl))
					&& (window.IsKeyDown(Keys.LeftShift) || window.IsKeyDown(Keys.RightShift)) &&
					window.IsKeyDown(Keys.F))
				{
					_projectContext.SearchingProject = true;
				}
			}

			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false)
			{
				if ((window.IsKeyDown(Keys.LeftControl) || window.IsKeyDown(Keys.RightControl))
					&& (window.IsKeyDown(Keys.LeftShift) || window.IsKeyDown(Keys.RightShift)) &&
					window.IsKeyDown(Keys.Q))
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
				if ((window.IsKeyDown(Keys.LeftControl) || window.IsKeyDown(Keys.RightControl))
					&& (window.IsKeyDown(Keys.LeftShift) || window.IsKeyDown(Keys.RightShift)) &&
					window.IsKeyDown(Keys.B))
				{
					_compiler.CompileScripts();
				}
			}

			if (_projectContext.CurrentProject != null && _compiler.BuildingGame == false
				&& _compiler.AssemblyLoaded && _compiler.AssemblyCompileErrors.Count == 0)
			{
				if ((window.IsKeyDown(Keys.LeftControl) || window.IsKeyDown(Keys.RightControl))
					&& (window.IsKeyDown(Keys.LeftShift) || window.IsKeyDown(Keys.RightShift)) &&
					window.IsKeyDown(Keys.G))
				{
					_compiler.BuildGame();
				}

				if ((window.IsKeyDown(Keys.LeftControl) || window.IsKeyDown(Keys.RightControl))
					&& (window.IsKeyDown(Keys.LeftShift) || window.IsKeyDown(Keys.RightShift)) &&
					window.IsKeyDown(Keys.R))
				{
					_compiler.BuildGame(true);
				}
			}
		}
	}
}
