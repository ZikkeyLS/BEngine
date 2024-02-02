
namespace BEngineEditor
{
	public class Scene : AssetData
	{
		public string SceneName = "New Scene";

		public Scene()
		{

		}

		public Scene(string sceneName)
		{
			SceneName = sceneName;
		}

		public Scene(string sceneName, Project project, string guid) : base(project, guid)
		{
			SceneName = sceneName;
		}
	}
}
