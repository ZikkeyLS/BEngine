using BEngine;

namespace BEngineCore
{
	public class ClientWindow : EngineWindow
	{
		private RuntimeProject _project;
		private FrameBuffer _game;

		protected override void OnLoad()
		{
			base.OnLoad();

			_game = graphics.CreateBuffer("Game", (uint)window.Size.X, (uint)window.Size.Y);

			_project = new(this);
			_project.LoadProject();
		}

		protected override void OnUpdate(double time)
		{
			base.OnUpdate(time);

			_project.LoadedScene?.CallEvent(EventID.Update);
		}

		protected override void OnRender(double time)
		{
			graphics.Render(this, (float)time, true);

			base.OnRender(time);
		}
	}
}
