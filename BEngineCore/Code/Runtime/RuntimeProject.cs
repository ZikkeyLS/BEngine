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

			Scene? test = TryLoadScene("73542132-8fd4-4995-b34d-99c5dd40e30c", true, false);

			// init AssetReader with.zip reader feature
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			StartRuntime();
		}
	}
}
