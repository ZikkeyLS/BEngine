
namespace BEngineCore.Code.Runtime
{
	internal class RuntimeProject : ProjectAbstraction
	{
		public override void LoadProjectData()
		{
			scripting.ReadScriptAssembly(Path.Combine(Directory.GetCurrentDirectory(), "ProjectAssembly.dll"));
			// init AssetReader with .zip reader feature
		}
	}
}
