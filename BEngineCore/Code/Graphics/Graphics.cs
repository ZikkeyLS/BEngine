﻿using Silk.NET.Input;
using Silk.NET.OpenGL;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
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
		-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
		 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
		 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
		 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
		-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
		-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

		-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
		 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
		 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
		-0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
		-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

		-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

		 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
		 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
		 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
		-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

		-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
		 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
		 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
		-0.5f,  0.5f, -0.5f,  0.0f, 1.0f
		};

		private Vector3[] cubePositions =
		{
			new Vector3( 0.0f, 0.0f, 0.0f),
			new Vector3( 2.0f, 5.0f, -15.0f),
			new Vector3(-1.5f, -2.2f, -2.5f),
			new Vector3(-3.8f, -2.0f, -12.3f),
			new Vector3( 2.4f, -0.4f, -3.5f),
			new Vector3(-1.7f, 3.0f, -7.5f),
			new Vector3( 1.3f, -2.0f, -2.5f),
			new Vector3( 1.5f, 2.0f, -2.5f),
			new Vector3( 1.5f, 0.2f, -1.5f),
			new Vector3(-1.3f, 1.0f, -1.5f)
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
			gl.Enable(GLEnum.DepthTest);

			gl.ClearColor(Color.CornflowerBlue);

			_camera = new Camera();

			_vao = gl.GenVertexArray();
			gl.BindVertexArray(_vao);

			_vbo = gl.GenBuffer();
			gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
			fixed (float* buff = vertices)
				gl.BufferData(GLEnum.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), buff, GLEnum.StaticDraw);

			_shader = new Shader("EngineData/Shaders/Shader.vert", "EngineData/Shaders/Shader.frag", gl);
			_texture = new Texture("EngineData/Textures/Bricks.jpg", gl);

			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 5 * sizeof(float), (void*)0);

			gl.EnableVertexAttribArray(2);
			gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));

			gl.BindVertexArray(0);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

			// For debug usage: gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
		}

		private Vector2? _lastMousePosition = null;

		public unsafe void Render(EngineWindow window, float time, bool forceRender = false)
		{
			gl.ClearColor(0f, 0f, 0f, 1.0f);
			gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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
					if (_lastMousePosition != null)
						difference = mouseMove - _lastMousePosition.Value;

					float senstivity = 0.05f * time;
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

				Matrix4x4 view = _camera.CalculateViewMatrix();
				Matrix4x4 projection = _camera.CalculateProjectionMatrix(frame.Width, frame.Height);

				int modelLoc = gl.GetUniformLocation(_shader.Program, "model");
				int viewLoc = gl.GetUniformLocation(_shader.Program, "view");
				int projLoc = gl.GetUniformLocation(_shader.Program, "projection");

				gl.BindVertexArray(_vao);

				_shader.Use();
				_texture.Bind();

				fixed (float* buff = view.GetRawMatrix())
					gl.UniformMatrix4(viewLoc, 1, false, buff);

				fixed (float* buff = projection.GetRawMatrix())
					gl.UniformMatrix4(projLoc, 1, false, buff);

				for (int i = 0; i < 10; i++)
				{
					Matrix4x4 model = new Matrix4x4();
					model = Matrix4x4.CreateTranslation(cubePositions[i]);
					model *= Matrix4x4.CreateFromYawPitchRoll(i * 0.1f, i * 0.1f, i * 0.1f);
					model *= Matrix4x4.CreateScale(new Vector3(i * 0.1f, i * 0.1f, i * 0.1f));

					fixed (float* buff = model.GetRawMatrix())
						gl.UniformMatrix4(modelLoc, 1, false, buff);


					gl.DrawArrays(GLEnum.Triangles, 0, 36);
				}


				gl.BindVertexArray(0);

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
