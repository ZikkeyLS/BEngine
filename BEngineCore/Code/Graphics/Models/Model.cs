using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace BEngineCore
{
	internal class Model
	{
		private List<Mesh> _meshes = new();
		private List<TextureMesh> _texturesLoaded = new();

		private string _directory;

		private Assimp assimp;
		private GL gl;

		public Model(string path)
		{
			assimp = Assimp.GetApi();
			gl = Graphics.gl;
			LoadModel(path);
		}

		public void Draw(Shader shader)
		{
			for (int i = 0; i < _meshes.Count; i++)
			{
				_meshes[i].Draw(shader);
			}
		}

		private unsafe void LoadModel(string path)
		{
			Silk.NET.Assimp.Scene* scene = assimp.ImportFile(path, (uint)(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));

			if (scene == null || scene->MFlags == (uint)SceneFlags.Incomplete || scene->MRootNode == null)
			{
				Logger.Main?.LogWarning("Cant load model with error: " + assimp.GetErrorStringS());
				return;
			}

			_directory = path.Substring(0, path.LastIndexOf("/") + 1);

			ProcessNode(scene->MRootNode, scene);
			assimp.ReleaseImport(scene);
		}

		private unsafe void ProcessNode(Node* node, Silk.NET.Assimp.Scene* scene)
		{
			for (int i = 0; i < node->MNumMeshes; i++)
			{
				Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
				_meshes.Add(ProcessMesh(mesh, scene));
			}

			for (int i = 0; i < node->MNumChildren; i++)
			{
				ProcessNode(node->MChildren[i], scene);
			}
		}

		private unsafe Mesh ProcessMesh(Silk.NET.Assimp.Mesh* mesh, Silk.NET.Assimp.Scene* scene)
		{
			List<VertexMesh> vertices = new();
			List<uint> indices = new();
			List<TextureMesh> textures = new();

			for (int i = 0; i < mesh->MNumVertices; i++)
			{
				VertexMesh vertex = new();

				vertex.Position.X = mesh->MVertices[i].X;
				vertex.Position.Y = mesh->MVertices[i].Y;
				vertex.Position.Z = mesh->MVertices[i].Z;

				vertex.Normal.X = mesh->MNormals[i].X;
				vertex.Normal.Y = mesh->MNormals[i].Y;
				vertex.Normal.Z = mesh->MNormals[i].Z;

				if (mesh->MTextureCoords[0] != null)
				{
					vertex.TexCoords.X = mesh->MTextureCoords[0][i].X;
					vertex.TexCoords.Y = mesh->MTextureCoords[0][i].Y;
				}
				else
				{
					vertex.TexCoords = Vector2.Zero;
				}

				vertices.Add(vertex);
			}

			for (int i = 0; i < mesh->MNumFaces; i++)
			{
				Face face = mesh->MFaces[i];
				for (int j = 0; j < face.MNumIndices; j++)
					indices.Add(face.MIndices[j]);
			}

			if (mesh->MMaterialIndex >= 0)
			{
				Material* material = scene->MMaterials[mesh->MMaterialIndex];

				List<TextureMesh> diffuseMaps = GetTextures(material, TextureType.Diffuse, "texture_diffuse");
				textures.AddRange(diffuseMaps);
				List<TextureMesh> specularMaps = GetTextures(material, TextureType.Specular, "texture_specular");
				textures.AddRange(specularMaps);
			}

			Mesh result = new Mesh(vertices, indices, textures);
			return result;
		}

		private unsafe List<TextureMesh> GetTextures(Material* material, TextureType type, string typeName)
		{
			List<TextureMesh> textures = new();

			for (uint i = 0; i < assimp.GetMaterialTextureCount(material, type); i++)
			{
				AssimpString path = new();
				TextureMapping mapping = new();
				uint uvIndex = 0;
				float blend = 0;
				TextureOp op = new();
				TextureMapMode mapMode = new();
				uint flags = 0;
				assimp.GetMaterialTexture(material, type, i, ref path, ref mapping, ref uvIndex, ref blend, ref op, ref mapMode, ref flags);

				bool skip = false;

				for (int j = 0; j < _texturesLoaded.Count; j++)
				{
					if (_texturesLoaded[j].Path == path)
					{
						textures.Add(_texturesLoaded[j]);
						skip = true;
						break;
					}
				}

				if (skip == false)
				{
					TextureMesh texture = new TextureMesh();
					texture.Texture = new Texture(_directory + path.AsString, gl);
					texture.ID = texture.Texture.ID;
					texture.Type = typeName;
					texture.Path = path.AsString;
					textures.Add(texture);
				}
			}

			return textures;
		}
	}
}
