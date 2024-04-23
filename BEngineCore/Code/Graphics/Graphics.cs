using BEngine;
using Silk.NET.OpenGL;
using System.Numerics;
using Color = System.Drawing.Color;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace BEngineCore
{
	public struct ModelRenderContext
	{
		public Model Model;
		public Transform Transform;
	}

	public class Graphics
	{
		public static GL gl { get; private set; }

		private EngineWindow _window;
		private Input _input;

		private Camera _camera;
		private Shader _shader;

		public List<ModelRenderContext> ModelsToRender = new();
		public HashSet<uint> TexturesToDelete = new();

		public Dictionary<string, FrameBuffer> FrameBuffers = new();

		public bool CameraOverride = false;

		private const int DefaultX = 1920;
		private const int DefaultY = 1080;

		public Graphics(GL gl)
		{
			Graphics.gl = gl;
		}

		public FrameBuffer CreateBuffer(string name, uint x, uint y)
		{
			FrameBuffer buffer = new FrameBuffer(x, y);
			FrameBuffers.Add(name, buffer);
			return buffer;
		}

		public unsafe void Initialize(EngineWindow window)
		{
			_window = window;
			_input = window.Input;

			gl.Enable(GLEnum.DepthTest);

			gl.Enable(GLEnum.CullFace);
			gl.CullFace(GLEnum.Back);

			gl.Enable(GLEnum.Blend);
			gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

			gl.ClearColor(Color.CornflowerBlue);

			_camera = new Camera();
			_shader = new Shader("EngineData/Shaders/Shader.vert", "EngineData/Shaders/Shader.frag", gl);

			// For debug usage: gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
		}

		private Vector2? _lastMousePosition = null;

		private bool fill = true;
		private DateTime fillTime = DateTime.Now;

		public unsafe void Render(float time, bool forceRender = false)
		{
			gl.ClearColor(0f, 0f, 0f, 1.0f);
			gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			if (TexturesToDelete.Count > 0)
			{
				foreach (uint textureID in TexturesToDelete)
					gl.DeleteTexture(textureID);
				TexturesToDelete.Clear();
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			if (_input.IsKeyPressed(Key.Escape) && (DateTime.Now - fillTime).TotalSeconds >= 0.25f)
			{
				if (fill)
				{
					gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
				}
				else
				{
					gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
				}

				fillTime = DateTime.Now;
				fill = !fill;
			}

			if (_camera.NativeCamera || !CameraOverride)
			{
				ProcessCameraMovement(_window, time);
			}

			_camera.Recalculate();

			foreach (FrameBuffer frame in FrameBuffers.Values)
			{
				frame.Bind();

				gl.ClearColor(Color.CornflowerBlue);
				gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				_shader.Use();

				Matrix4x4 view = _camera.CalculateViewMatrix();
				Matrix4x4 projection = _camera.CalculateProjectionMatrix(DefaultX, DefaultY);

				_shader.SetMatrix4("view", view);
				_shader.SetMatrix4("projection", projection);

				for (int i = 0; i < ModelsToRender.Count; i++)
				{
					Transform transform = ModelsToRender[i].Transform;

					Matrix4x4 model = Matrix4x4.CreateScale((Vector3)transform.Scale);
					model *= Matrix4x4.CreateFromQuaternion((Quaternion)transform.Rotation);
					Vector3 invertXPosition = new Vector3(transform.Position.x, transform.Position.y, transform.Position.z);
					model *= Matrix4x4.CreateTranslation(invertXPosition);
					_shader.SetMatrix4("model", model);

					ModelsToRender[i].Model.Draw(_shader);
				}
				ModelsToRender.Clear();

				frame.Unbind();
			}

			if (forceRender)
			{
				FrameBuffer main = FrameBuffers.First().Value;

				gl.ClearColor(Color.AliceBlue);
				gl.Clear(ClearBufferMask.ColorBufferBit);

				gl.BindFramebuffer(GLEnum.ReadFramebuffer, main.FBO);
				gl.FramebufferTexture2D(GLEnum.ReadFramebuffer, GLEnum.ColorAttachment0,
									   GLEnum.Texture2D, main.Texture, 0);
				gl.BlitFramebuffer(0, 0, (int)main.Width, (int)main.Height,
								  0, 0, (int)main.Width, (int)main.Height,
								  ClearBufferMask.ColorBufferBit, GLEnum.Linear);
				gl.BindFramebuffer(GLEnum.ReadFramebuffer, 0);
			}
		}

		public void AddCameraRequest(CameraHandlerRequest request)
		{
			_camera.AddRequest(request);
		}

		public void ResetCameraHandler()
		{
			_camera.ResetCameraHandler();
		}

		private void ProcessCameraMovement(EngineWindow window, float time)
		{
			float speed = time * 6;

			if (_input.IsKeyPressed(Key.W))
			{
				_camera.position += _camera.forward * speed;
			}
			if (_input.IsKeyPressed(Key.S))
			{
				_camera.position -= _camera.forward * speed;
			}
			if (_input.IsKeyPressed(Key.A))
			{
				_camera.position -= Vector3.Normalize(Vector3.Cross(_camera.forward, _camera.up)) * speed;
			}
			if (_input.IsKeyPressed(Key.D))
			{
				_camera.position += Vector3.Normalize(Vector3.Cross(_camera.forward, _camera.up)) * speed;
			}

			Vector2 difference = Vector2.Zero;
			Vector2 mouseMove = (Vector2)_input.GetMousePosition();

			if (_input.IsButtonPressed(MouseButton.Middle))
			{
				if (_input.GetCursorMode() != CursorMode.Raw)
				{
					_input.SetCursorMode(CursorMode.Raw);
					_lastMousePosition = mouseMove;
				}
				else
				{
					if (_lastMousePosition != null)
						difference = mouseMove - _lastMousePosition.Value;

					float senstivity = 0.1f;
					difference *= senstivity;
					_camera.rotation = (Quaternion)BEngine.Quaternion.FromEuler(BEngine.Quaternion.ToEuler(_camera.rotation) + new Vector3(difference.Y, -difference.X, 0));

					_lastMousePosition = mouseMove;
				}
			}
			else
			{
				if (_input.GetCursorMode() != CursorMode.Normal)
				{
					_input.SetCursorMode(CursorMode.Normal);
					_lastMousePosition = mouseMove;
				}
			}
		}
	}

	internal static class GraphicsUtils
	{
		public static Vector3 ToNativeProper(this BEngine.Vector3 vector)
		{
			return new Vector3(-vector.x, vector.y, vector.z);
		}
	}

	public static class MatrixUtils
	{
		public static float[] GetRawMatrix(this Matrix4x4 matrix)
		{
			return [
				matrix.M11,
				matrix.M12,
				matrix.M13,
				matrix.M14,
				matrix.M21,
				matrix.M22,
				matrix.M23,
				matrix.M24,
				matrix.M31,
				matrix.M32,
				matrix.M33,
				matrix.M34,
				matrix.M41,
				matrix.M42,
				matrix.M43,
				matrix.M44,
			];
		}
	}
}
