using Silk.NET.OpenGL;
using StbImageSharp;

namespace BEngineCore
{
	public class Texture : IDisposable
	{
		private uint _handle;
		private GL _gl;

		private int _maxSize = 4096;

		public uint ID => _handle;

		public unsafe Texture(byte[] data, GL gl)
		{
			ImageResult image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);

			if (image == null)
				throw new InvalidOperationException("Failed to load image from memory.");

			int properWidth = image.Width;
			int properHeight = image.Height;

			if (properWidth == properHeight && properWidth > _maxSize)
			{
				properWidth = _maxSize;
				properHeight = _maxSize;
			}
			else if (properWidth > _maxSize || properHeight > _maxSize)
			{
				if (properHeight > properWidth)
				{
					float aspect = (float)properWidth / properHeight;
					properHeight = _maxSize;
					properWidth = (int)(aspect * _maxSize);
				}
				else
				{
					float aspect = (float)properWidth / properHeight;
					properWidth = _maxSize;
					properHeight = (int)(aspect * _maxSize);
				}
			}

			int channels = (int)image.Comp;

			fixed (byte* buff = image.Data)
			{
				if (properWidth != image.Width || properHeight != image.Height)
				{
					StbImageResizeSharp.StbImageResize.stbir_resize_uint8(buff, image.Width, image.Height,
						image.Width * channels, buff, properWidth, properHeight, properWidth * channels, channels);
				}

				Load(gl, buff, (uint)properWidth, (uint)properHeight);
			}

			image.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private unsafe void Load(GL gl, void* data, uint width, uint height)
		{
			//Saving the gl instance.
			_gl = gl;

			//Generating the opengl handle;
			_handle = _gl.GenTexture();
			Bind();

			//Setting the data of a texture.
			_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			//Setting some texture perameters so the texture behaves as expected.
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

			//Generating mipmaps.
			_gl.GenerateMipmap(TextureTarget.Texture2D);
		}

		public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
		{
			//When we bind a texture we can choose which textureslot we can bind it to.
			_gl.ActiveTexture(textureSlot);
			_gl.BindTexture(TextureTarget.Texture2D, _handle);
		}

		public void Dispose()
		{
			if (ProjectAbstraction.LoadedProject != null)
				ProjectAbstraction.LoadedProject.Graphics.TexturesToDelete.Add(_handle);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}