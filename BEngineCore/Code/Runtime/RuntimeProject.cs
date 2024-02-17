using BEngine;

namespace BEngineCore
{
	internal class RuntimeProject : ProjectAbstraction
	{
		public override void LoadProjectData()
		{
			logger.EnableFileLogs = true;

			scripting.ReadScriptAssembly(AppDomain.CurrentDomain.FriendlyName + "Assembly.dll");
			reader = new AssetReader("Game.data", AssetReaderType.Archive);
			Scene? test = TryLoadScene("7976e5ca-d6d7-4af7-a89e-d92aba20c946", true, false);

			// init AssetReader with.zip reader feature
		}
	}
}
