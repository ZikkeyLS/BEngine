using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;

namespace BEngineCore
{
	public class Window
	{
		protected GameWindow _window;

		public Window(string title = "Window", int x = 1280, int y = 720)
		{
			GameWindowSettings gameSettings = GameWindowSettings.Default;
			NativeWindowSettings nativeWindowSettings = NativeWindowSettings.Default;
			nativeWindowSettings.Title = title;
			nativeWindowSettings.ClientSize = new OpenTK.Mathematics.Vector2i(x, y);

			_window = new GameWindow(gameSettings, nativeWindowSettings);

			_window.MouseWheel += MouseWheel;
			_window.TextInput += OnTextInput;
			_window.Load += OnLoad;
			_window.Resize += OnResize;
			_window.RenderFrame += OnRenderFrame;
		}

		protected virtual void MouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj) { }
		protected virtual void OnTextInput(OpenTK.Windowing.Common.TextInputEventArgs obj) { }
		protected virtual void OnResize() { }
		protected virtual void OnLoad() { }
		protected virtual void OnPreRender(float time) { }
		protected virtual void OnRender() { }
		protected virtual void OnPostRender() { }

		private void OnResize(OpenTK.Windowing.Common.ResizeEventArgs obj)
		{
			GL.Viewport(0, 0, _window.ClientSize.X, _window.ClientSize.Y);

			OnResize();
		}

		private void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
		{
			OnPreRender((float)obj.Time);

			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			OnRender();

			OnPostRender();

			_window.SwapBuffers();
		}

		public void Run()
		{
			_window.Run();
		}
	}
}
