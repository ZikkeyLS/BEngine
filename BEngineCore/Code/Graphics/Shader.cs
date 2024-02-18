using Silk.NET.OpenGL;

namespace BEngineCore
{
	internal class Shader
	{
		public uint Program { get; private set; }

		protected GL gl;


		public Shader(string vertPath, string fragPath, GL gl)
		{
			this.gl = gl;

			uint vertexShader = gl.CreateShader(GLEnum.VertexShader);
			gl.ShaderSource(vertexShader, File.ReadAllText(vertPath));
			gl.CompileShader(vertexShader);

			gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
			if (vStatus != (int)GLEnum.True)
				Console.WriteLine("Vertex shader failed to compile: " + gl.GetShaderInfoLog(vertexShader));

			uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
			gl.ShaderSource(fragmentShader, File.ReadAllText(fragPath));
			gl.CompileShader(fragmentShader);

			gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
			if (fStatus != (int)GLEnum.True)
				Console.WriteLine("Fragment shader failed to compile: " + gl.GetShaderInfoLog(fragmentShader));

			Program = gl.CreateProgram();
			gl.AttachShader(Program, vertexShader);
			gl.AttachShader(Program, fragmentShader);
			gl.LinkProgram(Program);

			gl.GetProgram(Program, ProgramPropertyARB.LinkStatus, out int lStatus);
			if (lStatus != (int)GLEnum.True)
				Console.WriteLine("Program failed to link: " + gl.GetProgramInfoLog(Program));

			gl.DetachShader(Program, vertexShader);
			gl.DetachShader(Program, fragmentShader);
			gl.DeleteShader(vertexShader);
			gl.DeleteShader(fragmentShader);
		}

		public void SetFloat(string attributeName, float value)
		{

		}

		public void Use()
		{
			gl.UseProgram(Program);
		}
	}
}
