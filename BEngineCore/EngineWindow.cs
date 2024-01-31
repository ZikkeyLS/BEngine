using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace BEngineCore
{
	public class EngineWindow
	{
		protected IWindow window;
		protected IInputContext input;

		protected GL gl;

		public EngineWindow(string title = "Window", int x = 1280, int y = 720)
		{
			WindowOptions options = WindowOptions.Default;
			options.Title = title;
			options.Size = new Vector2D<int>(x, y);
			window = Window.Create(options);

			window.Load += OnLoad;
			window.Render += OnRender;
			window.Update += OnUpdate;
			window.Resize += OnResize;
			window.FramebufferResize += OnFramebufferResize;
			window.Closing += OnClose;
		}

		public void Run()
		{
			window.Run();
		}

		protected virtual void OnLoad() 
		{
			input = window.CreateInput();

			input.Keyboards[0].KeyChar += OnTextInput;
			input.Mice[0].Scroll += OnScroll;

			gl = window.CreateOpenGL();
		}

		protected virtual void OnRender(double time) { }

		protected virtual void OnUpdate(double time) { }

		protected virtual void OnResize(Vector2D<int> size) { }

		private void OnFramebufferResize(Vector2D<int> obj)
		{
			gl.Viewport(obj);
		}

		protected virtual void OnClose() { }

		protected virtual void OnTextInput(IKeyboard arg1, char arg2) { }

		protected virtual void OnScroll(IMouse arg1, ScrollWheel arg2) { }
	}
}
