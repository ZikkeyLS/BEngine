
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace BEngineCore
{
	public class EngineWindow
	{
		protected IWindow _window;
		protected IInputContext _inputs;

		public EngineWindow(string title = "Window", int x = 1280, int y = 720)
		{
			WindowOptions options = WindowOptions.Default;
			options.Title = title;
			options.Size = new Vector2D<int>(x, y);
			_window = Window.Create(options);

			_window.Load += OnLoad;
			_window.Render += OnRender;
			_window.Update += OnUpdate;
			_window.Resize += OnResize;
			_window.Closing += OnClose;
		}

		public void Run()
		{
			_window.Run();
		}

		protected virtual void OnLoad() 
		{
			_inputs = _window.CreateInput();

			_inputs.Keyboards[0].KeyChar += OnTextInput;
			_inputs.Mice[0].Scroll += OnScroll;
		}

		protected virtual void OnRender(double time) { }

		protected virtual void OnUpdate(double time) { }

		protected virtual void OnResize(Vector2D<int> size) { }

		protected virtual void OnClose() { }

		protected virtual void OnTextInput(IKeyboard arg1, char arg2) { }

		protected virtual void OnScroll(IMouse arg1, ScrollWheel arg2) { }
	}
}
