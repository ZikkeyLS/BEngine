using Silk.NET.OpenGL;

namespace BEngineCore
{
	public unsafe class FrameBuffer : IDisposable
	{
		private uint _fbo;
		private uint _texture;
		private uint _rbo;

		public uint FBO => _fbo;
		public uint Texture => _texture;

		private GL gl => Graphics.gl;

		public uint Width { get; private set; }
		public uint Height { get; private set; }

		public FrameBuffer(uint width, uint height)
		{
			Width = width;
			Height = height;

			gl.GenFramebuffers(1, out _fbo);
			gl.BindFramebuffer(GLEnum.Framebuffer, _fbo);

			gl.GenTextures(1, out _texture);
			gl.BindTexture(GLEnum.Texture2D, _texture);

			gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, GLEnum.UnsignedByte, null);
			gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
			gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
			gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, _texture, 0);

			gl.GenRenderbuffers(1, out _rbo);
			gl.BindRenderbuffer(GLEnum.Renderbuffer, _rbo);
			gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, width, height);
			gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, _rbo);

			if (gl.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
			{
				// TODO: log error
			}

			gl.BindFramebuffer(GLEnum.Framebuffer, 0);
			gl.BindTexture(GLEnum.Texture, 0);
			gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);
		}

		public void Dispose()
		{
			gl.DeleteFramebuffers(1, _fbo);
			gl.DeleteTextures(1, _texture);
			gl.DeleteRenderbuffers(1, _rbo);
		}

		public uint GetFrameTexture()
		{
			return _texture;
		}

		public void RescaleFrameBuffer(uint width, uint height)
		{
			Width = width;
			Height = height;

			gl.BindTexture(GLEnum.Texture2D, _texture);
			gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, GLEnum.UnsignedByte, null);
			gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
			gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
			gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, _texture, 0);

			gl.BindRenderbuffer(GLEnum.Renderbuffer, _rbo);
			gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, width, height);
			gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, _rbo);
		}

		public void Bind()
		{
			gl.BindFramebuffer(GLEnum.Framebuffer, _fbo);
		}

		public void Unbind()
		{
			gl.BindFramebuffer(GLEnum.Framebuffer, 0);
		}
	}
}
