using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace BEngineCore
{
	public class EngineWindow
	{
		protected IWindow window;
		protected IInputContext inputContext;
		protected Input input;

		protected GL gl;
		protected Graphics graphics;

		public Graphics Graphics => graphics;
		public Input Input => input;

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

		public bool IsKeyPressed(Key key)
		{
			return inputContext.Keyboards[0].IsKeyPressed(key);
		}

		public bool IsMouseButtonPressed(MouseButton button)
		{
			return inputContext.Mice[0].IsButtonPressed(button);
		}

		public void SetCursorMode(CursorMode mode)
		{
			if (inputContext.Mice[0].Cursor.IsSupported(mode))
			{
				inputContext.Mice[0].Cursor.CursorMode = mode;
			}
		}

		public CursorMode GetCursorMode()
		{
			return inputContext.Mice[0].Cursor.CursorMode;
		}

		public Vector2 GetMousePosition()
		{
			return inputContext.Mice[0].Position;
		}

		protected virtual void OnLoad() 
		{
			inputContext = window.CreateInput();

			inputContext.Keyboards[0].KeyChar += OnTextInput;
			inputContext.Mice[0].Scroll += OnScroll;

			input = new Input(inputContext);

			gl = window.CreateOpenGL();
			graphics = new(gl);
			graphics.Initialize();
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
