using Silk.NET.Input;
using Silk.NET.Windowing;

namespace BEngineCore
{
	public class ClientWindow : EngineWindow
	{
		protected override void OnLoad()
		{
			base.OnLoad();
		}

		protected override void OnRender(double time)
		{
			base.OnRender(time);
			_window.SwapBuffers();
		}
	}
}
