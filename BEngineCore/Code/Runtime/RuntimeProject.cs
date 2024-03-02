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
			Scene? test = TryLoadScene("a736c7ae-d289-43e0-b865-558b3f4df5f9", true, false);

			// init AssetReader with.zip reader feature
		}

		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			LoadedScene?.CallEvent(EventID.Start);
		}
	}
}
