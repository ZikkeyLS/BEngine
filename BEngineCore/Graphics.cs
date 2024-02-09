using Silk.NET.OpenGL;
using Color = System.Drawing.Color;

namespace BEngineCore
{
	public class Graphics
	{
		public static GL gl { get; private set; }

		private Shader _shader;

		private uint _vao;
		private uint _vbo;
		private uint _ebo;
		private Texture _texture;

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

			_shader = new Shader("EngineData/Shaders/Shader.vert", "EngineData/Shaders/Shader.frag", gl);
			_texture = new Texture("EngineData/Textures/Bricks.jpg", gl);

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

		public unsafe void Render(float time)
		{
			gl.ClearColor(0f, 0f, 0f, 1.0f);
			gl.Clear(ClearBufferMask.ColorBufferBit);

			foreach (FrameBuffer frame in FrameBuffers.Values)
			{
				frame.Bind();

				gl.ClearColor(Color.CornflowerBlue);
				gl.Clear(ClearBufferMask.ColorBufferBit);

				gl.BindVertexArray(_vao);
				_texture.Bind();
				_shader.Use();
				gl.DrawElements(PrimitiveType.Triangles, 6, GLEnum.UnsignedInt, 0);
				gl.BindVertexArray(0);

				frame.Unbind();
			}
		}
	}
}
