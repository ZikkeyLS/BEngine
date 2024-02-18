using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace BEngineCore
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexMesh
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 TexCoords;
	}

	public struct TextureMesh
	{
		public uint ID;
		public string Type;
	}

	internal class Mesh
	{
		public readonly VertexMesh[] Vertices;
		public readonly uint[] Indices;
		public readonly TextureMesh[] Textures;

		private uint _vao;
		private uint _vbo;
		private uint _ebo;

		public GL gl => Graphics.gl;
		
		public Mesh(List<VertexMesh> vertices, List<uint> indices, List<TextureMesh> textures)
		{
			Vertices = vertices.ToArray();
			Indices = indices.ToArray();
			Textures = textures.ToArray();

			SetupMesh();
		}

		public void Draw(Shader shader)
		{
			uint diffuseNr = 1;
			uint specularNr = 1;

			for (uint i = 0; i < Textures.Length; i++)
			{
				gl.ActiveTexture((GLEnum)((uint)GLEnum.Texture0 + i));

				StringBuilder stringStream = new StringBuilder();

				string number;
				string name = Textures[i].Type;

				if (name == "texture_diffuse")
				{
					stringStream.Append(diffuseNr++);
				}
				else if (name == "texture_specular")
				{
					stringStream.Append(specularNr++);
				}

				number = stringStream.ToString();

				
			}
		}

		private unsafe void SetupMesh()
		{
			_vao = gl.GenVertexArray();
			_vbo = gl.GenBuffer();
			_ebo = gl.GenBuffer();

			gl.BindVertexArray(_vao);

			gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
			fixed (void* ptr = Vertices)
			{
				gl.BufferData(GLEnum.ArrayBuffer, (nuint)(sizeof(VertexMesh) * Vertices.Length), ptr, GLEnum.StaticDraw);
			}

			gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);
			fixed (void* ptr = Indices.ToArray())
			{
				gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(sizeof(uint) * Indices.Length), ptr, GLEnum.StaticDraw);
			}

			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)sizeof(VertexMesh), (void*)0);

			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(1, 3, GLEnum.Float, false, (uint)sizeof(VertexMesh), (void*)sizeof(Vector3));

			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(2, 2, GLEnum.Float, false, (uint)sizeof(VertexMesh), (void*)(sizeof(Vector3) * 2));

			gl.BindVertexArray(0);
		}
	}
}
