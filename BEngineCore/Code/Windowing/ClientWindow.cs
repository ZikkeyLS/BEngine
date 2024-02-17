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

			graphics.CreateBuffer((uint)window.Size.X, (uint)window.Size.Y);

			_project = new();
			_project.LoadProject();
		}

		protected override void OnRender(double time)
		{
			graphics.Render(this, (float)time);
			base.OnRender(time);
		}
	}
}
