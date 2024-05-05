using BEngine;
using Silk.NET.SDL;

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
			ProjectRuntimeInfo? projectRuntimeInfo = JsonUtils.Deserialize<ProjectRuntimeInfo>(reader.Packer.ReadFile("Game.data", "ProjectRuntimeInfo.json"));
			if(projectRuntimeInfo != null)
			{
				TryLoadScene(projectRuntimeInfo.RuntimeScenes.First().Value.GUID, true, false);
			}
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			StartRuntime();
		}
	}
}
