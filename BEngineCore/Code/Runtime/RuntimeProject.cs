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
			reader = new AssetReader(["./"], AssetReaderType.Directory);

			Scene? test = TryLoadScene("a5dc155e-2e0c-4b0d-952d-33bcee4c57a7", true, false);

			// init AssetReader with.zip reader feature
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			StartRuntime();
			LoadedScene?.CallEvent(EventID.Start);
		}
	}
}
