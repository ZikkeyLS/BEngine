using BEngineCore.Code.Runtime;

namespace BEngineCore
{
	public class ClientWindow : EngineWindow
	{
		private RuntimeProject _project;

		protected override void OnLoad()
		{
			base.OnLoad();

			Scripting.LoadInternalScriptingAPI();

			_project = new();
			_project.LoadProject();
		}

		protected override void OnRender(double time)
		{
			base.OnRender(time);
		}
	}
}
