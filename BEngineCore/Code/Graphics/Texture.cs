using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.CompilerServices;

namespace BEngineCore
{
	public class Texture : IDisposable
	{
		private uint _handle;
		private GL _gl;

		private int _maxSize = 2048;
		private byte[] _pixelBytes;

		public uint ID => _handle;

		public unsafe Texture(string path, GL gl, FlipMode fm = FlipMode.None)
		{
			Image<Rgba32> img = Image.Load<Rgba32>(path);

			int properWidth = img.Width;
			int properHeight = img.Height;

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

			img.Mutate(x =>
			{
				x.Flip(fm);
				x.Resize(properWidth, properHeight);
			});

			_pixelBytes = new byte[img.Width * img.Height * Unsafe.SizeOf<Rgba32>()];
			img.CopyPixelDataTo(_pixelBytes);

			fixed (void* buff = _pixelBytes)
			{
				Load(gl, buff, (uint)img.Width, (uint)img.Height);
			}

			img.Dispose();
			img = null;
			GC.Collect();
		}

		public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
		{
			//We want the ability to create a texture using data generated from code aswell.
			fixed (void* d = &data[0])
			{
				Load(gl, d, width, height);
			}
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
			//In order to dispose we need to delete the opengl handle for the texure.
			_gl.DeleteTexture(_handle);
		}
	}
}