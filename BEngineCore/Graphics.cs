using Silk.NET.Input;
using Silk.NET.OpenGL;
using System.Numerics;
using Color = System.Drawing.Color;

namespace BEngineCore
{
	public class Graphics
	{
		public static GL gl { get; private set; }

		private Camera _camera;
		private Shader _shader;
		private Texture _texture;

		private uint _vao;
		private uint _vbo;
		private uint _ebo;

		private float[] vertices = {
    // Позиции          // Цвета             // Текстурные координаты
     0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // Верхний правый
     0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // Нижний правый
    -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // Нижний левый
    -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // Верхний левый
		};

		private uint[] indices = {
			0, 1, 3, // First Triangle
			1, 2, 3  // Second Triangle
		};

		public Dictionary<string, FrameBuffer> FrameBuffers = new();

		public Graphics(GL gl)
		{
			Graphics.gl = gl;
		}

		public FrameBuffer CreateBuffer(uint x, uint y)
		{
			FrameBuffer buffer = new FrameBuffer(x, y);
			FrameBuffers.Add(Guid.NewGuid().ToString(), buffer);
			return buffer;
		}

		public unsafe void Initialize()
		{
			gl.ClearColor(Color.CornflowerBlue);

			_camera = new Camera();

			_vao = gl.GenVertexArray();
			gl.BindVertexArray(_vao);

			_vbo = gl.GenBuffer();
			gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
			fixed (float* buff = vertices)
				gl.BufferData(GLEnum.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), buff, GLEnum.StaticDraw);

			_ebo = gl.GenBuffer();
			gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);
			fixed (uint* buff = indices)
				gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(sizeof(uint) * indices.Length), buff, GLEnum.StaticDraw);

			_shader = new Shader("EngineData/Shaders/Shader.vert", "EngineData/Shaders/Shader.frag", gl);
			_texture = new Texture("EngineData/Textures/Bricks.jpg", gl);

			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)0);

			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));

			gl.EnableVertexAttribArray(2);
			gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));

			gl.BindVertexArray(0);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

			// For debug usage: gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
		}

		private Vector2 _lastMousePosition = Vector2.One * -1;

		public unsafe void Render(EngineWindow window, float time)
		{
			gl.ClearColor(0f, 0f, 0f, 1.0f);
			gl.Clear(ClearBufferMask.ColorBufferBit);

			float speed = time;

			if (window.IsKeyPressed(Key.W))
			{
				_camera.position += speed * _camera.forward;
			}
			if (window.IsKeyPressed(Key.S))
			{
				_camera.position -= speed * _camera.forward;
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
					if (_lastMousePosition != Vector2.One * -1)
						difference = mouseMove - _lastMousePosition;

					float senstivity = 0.05f * time;
					difference *= senstivity;

					_camera.x -= difference.X;
					_camera.y += difference.Y;

					_camera.Recalculate();

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

			foreach (FrameBuffer frame in FrameBuffers.Values)
			{
				frame.Bind();

				gl.ClearColor(Color.CornflowerBlue);
				gl.Clear(ClearBufferMask.ColorBufferBit);

				//Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, cameraDirection));
				//Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);

				Matrix4x4 view = _camera.CalculateViewMatrix();
				Matrix4x4 projection = _camera.CalculateProjectionMatrix(frame.Width, frame.Height);

				int modelLoc = gl.GetUniformLocation(_shader.Program, "model");
				int viewLoc = gl.GetUniformLocation(_shader.Program, "view");
				int projLoc = gl.GetUniformLocation(_shader.Program, "projection");

				fixed (float* buff = model.GetRawMatrix())
					gl.UniformMatrix4(modelLoc, 1, false, buff);

				fixed (float* buff = view.GetRawMatrix())
					gl.UniformMatrix4(viewLoc, 1, false, buff);

				fixed (float* buff = projection.GetRawMatrix())
					gl.UniformMatrix4(projLoc, 1, false, buff);

				gl.BindVertexArray(_vao);
				_shader.Use();
				_texture.Bind();
				gl.DrawElements(PrimitiveType.Triangles, 6, GLEnum.UnsignedInt, (void*)0);
				gl.BindVertexArray(0);

				frame.Unbind();
			}
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
