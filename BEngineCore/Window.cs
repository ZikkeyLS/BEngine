using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using System;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace BEngineCore
{
	public class Window
	{
		private GameWindow _window;
		private ImGuiController _controller;

		private double time = 0;

		public Action OnRenderUI;

		public Window()
		{
			_window = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);

			_window.Load += OnLoad;
			_window.Resize += OnResize;
			_window.RenderFrame += OnRenderFrame;
		}

		private void OnResize(OpenTK.Windowing.Common.ResizeEventArgs obj)
		{
			GL.Viewport(0, 0, _window.ClientSize.X, _window.ClientSize.Y);
			_controller.WindowResized(_window.ClientSize.X, _window.ClientSize.Y);
		}

		private void OnLoad()
		{
			_controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
		}

		private void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
		{
			_controller.Update(_window, (float)obj.Time);

			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			if (OnRenderUI.GetInvocationList().Length != 0)
				OnRenderUI.Invoke();

			_controller.Render();
			ImGuiController.CheckGLError("End of frame");

			_window.SwapBuffers();
		}

		public void SubscribeToRender(Action action)
		{
			_window.RenderFrame += (e) => 
			{ time = e.Time; action.Invoke(); };
		}

		public void Run()
		{
			_window.Run();
		}

		public static void Initialize(string basePath)
		{
			string saveCurDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binary"));

			var dll = Assembly.LoadFile(basePath);
			foreach (Type type in dll.GetExportedTypes())
			{
				if (type.Name == nameof(InternalCalls))
				{
					var c = Activator.CreateInstance(type);
					type.InvokeMember("Test", BindingFlags.InvokeMethod, null, c, null);
				}
			}
		}
	}
}
