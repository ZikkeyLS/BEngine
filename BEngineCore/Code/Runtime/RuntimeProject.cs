using BEngine;

namespace BEngineCore
{
	internal class RuntimeProject : ProjectAbstraction
	{
		public RuntimeProject(EngineWindow window) : base(window)
		{

		}

		public override void LoadProjectData()
		{
			logger.EnableFileLogs = true;
			logger.ClearFileLogs();

			scripting.ReadScriptAssembly(AppDomain.CurrentDomain.FriendlyName + "Assembly.dll");
			reader = new AssetReader(["./"], ["Game.data"]);

			if (reader.Packer == null)
				return;

			string? vertData = reader.ShaderContext.GetShaderData("EngineData/Assets/Shaders/Shader.vert.shader");
			string? fragData = reader.ShaderContext.GetShaderData("EngineData/Assets/Shaders/Shader.frag.shader");

			if (vertData != null && fragData != null)
			{
				graphics.SetMainShader(vertData, fragData);
			}

			Stream? runtimeInfo = reader.Packer.ReadFile("Game.data", "ProjectRuntimeInfo.json");
			if (runtimeInfo == null)
				return;

			ProjectRuntimeInfo? projectRuntimeInfo = JsonUtils.Deserialize<ProjectRuntimeInfo>(runtimeInfo);
			if (projectRuntimeInfo != null)
			{
				TryLoadScene(projectRuntimeInfo.RuntimeScenes.First().GUID, true, false);
			}
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			StartRuntime();
		}
	}
}
