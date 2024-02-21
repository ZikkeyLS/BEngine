using BEngine;
using Silk.NET.Input;
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
		public static Thread MainThread { get; private set; }

		private Camera _camera;
		private Shader _shader;

		// private Model _model;
		public List<ModelRenderContext> ModelsToRender = new();


		public Dictionary<string, FrameBuffer> FrameBuffers = new();

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

		public unsafe void Initialize()
		{
			MainThread = Thread.CurrentThread;

			gl.Enable(GLEnum.DepthTest);

			gl.Enable(GLEnum.CullFace);
			gl.CullFace(GLEnum.Back);

		//	gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
	//		gl.Enable(GLEnum.Blend);

			gl.ClearColor(Color.CornflowerBlue);

			_camera = new Camera();
			_shader = new Shader("EngineData/Shaders/Shader.vert", "EngineData/Shaders/Shader.frag", gl);

			//_model = new Model("EngineData/Models/Rifle/M1 Garand Lowpoly.fbx");
			//_model = new Model("EngineData/Models/Rifle/M1 Garand.obj");

			// For debug usage: gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
		}

		private Vector2? _lastMousePosition = null;

		private bool fill = true;
		private DateTime fillTime = DateTime.Now;

		public unsafe void Render(EngineWindow window, float time, bool forceRender = false)
		{
			gl.ClearColor(0f, 0f, 0f, 1.0f);
			gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			if (window.IsKeyPressed(Key.Escape) && (DateTime.Now - fillTime).TotalSeconds >= 0.25f)
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

			float speed = time * 6;
			if (window.IsKeyPressed(Key.W))
			{
				_camera.position += _camera.forward * speed;
			}
			if (window.IsKeyPressed(Key.S))
			{
				_camera.position -= _camera.forward * speed;
			}
			if (window.IsKeyPressed(Key.A))
			{
				_camera.position -= Vector3.Normalize(Vector3.Cross(_camera.forward, _camera.up)) * speed;
			}
			if (window.IsKeyPressed(Key.D))
			{
				_camera.position += Vector3.Normalize(Vector3.Cross(_camera.forward, _camera.up)) * speed;
			}

			Vector2 difference = Vector2.Zero;
			Vector2 mouseMove = window.GetMousePosition();

			if (window.IsMouseButtonPressed(MouseButton.Middle))
			{
				if (window.GetCursorMode() != CursorMode.Raw)
				{
					window.SetCursorMode(CursorMode.Raw);
					_lastMousePosition = mouseMove;
				}
				else
				{
					if (_lastMousePosition != null)
						difference = mouseMove - _lastMousePosition.Value;

					float senstivity = 0.002f;
					difference *= senstivity;

					_camera.rotation.X += difference.X;
					_camera.rotation.Y += difference.Y;

					_lastMousePosition = mouseMove;
				}	
			}
			else
			{
				if (window.GetCursorMode() != CursorMode.Normal)
				{
					window.SetCursorMode(CursorMode.Normal);
					_lastMousePosition = mouseMove;
				}
			}

			_camera.Recalculate();

			foreach (FrameBuffer frame in FrameBuffers.Values)
			{
				frame.Bind();

				gl.ClearColor(Color.CornflowerBlue);
				gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				_shader.Use();

				Matrix4x4 view = _camera.CalculateViewMatrix();
				Matrix4x4 projection = _camera.CalculateProjectionMatrix(frame.Width, frame.Height);

				_shader.SetMatrix4("view", view);
				_shader.SetMatrix4("projection", projection);

				//_model.Draw(_shader);

				for (int i = 0; i < ModelsToRender.Count; i++)
				{
					Transform transform = ModelsToRender[i].Transform;

					Matrix4x4 model = Matrix4x4.CreateScale(transform.Scale.ToNative());
					model *= Matrix4x4.CreateFromQuaternion(transform.Rotation.ToNative());
					model *= Matrix4x4.CreateTranslation(transform.Position.ToNative());
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


	}

	internal static class GraphicsUtils
	{
		public static Vector3 ToNative(this BEngine.Vector3 vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}

		public static Quaternion ToNative(this BEngine.Quaternion vector)
		{
			return new Quaternion(vector.x, vector.y, vector.z, vector.w);
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
