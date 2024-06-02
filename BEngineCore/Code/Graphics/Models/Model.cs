using BEngineCore;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;

public struct AdditionalData
{
	public string Path;
	public string AdditionalPath;
	public AssetType AssetType;

}

namespace BEngineCore
{
	public class Model : IDisposable
	{
		private List<Mesh> _meshes = new();
		private List<TextureMesh> _texturesLoaded = new();

		private string _directory = string.Empty;

		private Assimp assimp;
		private GL gl;

		public Model(AssetMetaData asset)
		{
			gl = Graphics.gl;
			LoadModel(asset);
		}

		public void Draw(Shader shader)
		{
			for (int i = 0; i < _meshes.Count; i++)
			{
				_meshes[i].Draw(shader);
			}
		}

		private unsafe void LoadModel(AssetMetaData asset)
		{
			assimp = Assimp.GetApi();
			var data = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetBytes(asset);

			if (data == null)
				return;

			fixed (byte* fileData = data)
			{
				using var io = new CustomFileIO
				(
					(file, access, mode) =>
					{
						Stream? compressed = null;
						if (asset.AssetType == AssetType.ArchieveFile)
						{
							AssetMetaData? findAsset = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetByPath(file.ToString());
							if (findAsset != null)
							{
								compressed = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetStream(findAsset);
							}
						}
						else
						{
							AssetMetaData? findAsset = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetByPath(file.ToString());
							if (findAsset != null)
							{
								compressed = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetStream(findAsset);
							}
						}

						var ms = new MemoryStream();

						if (compressed == null)
						{
							Logger.Main?.LogMessage("Something went wrong");
							return ms;
						}
						compressed.CopyTo(ms);
						ms.Seek(0, SeekOrigin.Begin);
						compressed.Close();
						return ms;
					}
				);

				Silk.NET.Assimp.Scene* scene = assimp.ImportFileEx(asset.GetAssetPath(), (uint)(PostProcessSteps.Triangulate
					| PostProcessSteps.FlipUVs | PostProcessSteps.JoinIdenticalVertices
					| PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GenerateSmoothNormals), ref io.FileIO);

				if (scene == null || scene->MFlags == (uint)SceneFlags.Incomplete || scene->MRootNode == null)
				{
					Logger.Main?.LogWarning("Cant load model with error: " + assimp.GetErrorStringS());
					return;
				}

				_directory = asset.GetAssetPath().Substring(0, asset.GetAssetPath().LastIndexOf("/") + 1);
				if (_directory == string.Empty)
					_directory = asset.GetAssetPath().Substring(0, asset.GetAssetPath().LastIndexOf("\\") + 1);

				ProcessNode(scene->MRootNode, scene);
				assimp.ReleaseImport(scene);
			}

			assimp.Dispose();
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

				if (mesh->MNormals != null)
				{
					vertex.Normal.X = mesh->MNormals[i].X;
					vertex.Normal.Y = mesh->MNormals[i].Y;
					vertex.Normal.Z = mesh->MNormals[i].Z;
				}

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

				//for (int i = 0; i < material->MNumProperties; i++)
				//{
				//	Console.WriteLine(material->MProperties[i]->MKey.AsString);
				//	Console.WriteLine(material->MProperties[i]->MIndex);
				//	Console.WriteLine(material->MProperties[i]->MSemantic);
				//	Console.WriteLine(material->MProperties[i]->MType);
				//}

				List<TextureMesh> diffuseMaps = GetTextures(material, TextureType.Diffuse, "texture_diffuse");
				if (diffuseMaps.Count == 0)
				{
					diffuseMaps.Add(GetDefaultTexture("texture_diffuse"));
				}
				textures.AddRange(diffuseMaps);

				List<TextureMesh> specularMaps = GetTextures(material, TextureType.Specular, "texture_specular");
				textures.AddRange(specularMaps);

			}

			Mesh result = new Mesh(vertices, indices, textures);
			return result;
		}

		public Texture? _defaultLoaded = null;
		private unsafe TextureMesh GetDefaultTexture(string typeName)
		{
			TextureMesh texture = new TextureMesh();
			Texture? load = null;

			if (_defaultLoaded != null)
			{
				load = _defaultLoaded;
			}
			else
			{
				AssetMetaData? defaultImage = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetByPath("EngineData/Assets/Textures/Default.jpg");

				if (defaultImage != null)
				{
					byte[]? data = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetBytes(defaultImage);
					if (data != null)
					{
						load = new Texture(data, gl);
						_defaultLoaded = load;
					}
				}
			}

			if (load == null)
			{
				return texture;
			}

			texture.Texture = load;
			texture.ID = texture.Texture.ID;
			texture.Type = typeName;
			texture.Path = "EngineData/Assets/Textures/Default.jpg";
			return texture;
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

				string resultPath = _directory + path.AsString;
				if (resultPath.Contains(".fbm"))
				{
					skip = true;
				}

				if (skip == false && ProjectAbstraction.LoadedProject.AssetsReader.HasAsset(resultPath))
				{
					AssetMetaData? asset = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetByPath(resultPath);

					if (asset == null)
					{
						string modelPath = path.AsString;
						modelPath = "t" + modelPath.Substring(1);
						resultPath = _directory + modelPath;

						asset = ProjectAbstraction.LoadedProject.AssetsReader.GetAssetByPath(resultPath);
					}

					if (asset != null)
					{
						TextureMesh texture = new TextureMesh();
						texture.Texture = new Texture(ProjectAbstraction.LoadedProject.AssetsReader.GetAssetBytes(asset), gl);
						texture.ID = texture.Texture.ID;
						texture.Type = typeName;
						texture.Path = path.AsString;
						textures.Add(texture);
						_texturesLoaded.Add(texture);
					}
				}
			}

			return textures;
		}

		public void Dispose()
		{
			for (int i = 0; i < _meshes.Count; i++)
			{
				_meshes[i].Dispose();
			}
		}
	}
}
