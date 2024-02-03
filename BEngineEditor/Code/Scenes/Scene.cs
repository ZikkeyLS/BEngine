using BEngineScripting;

namespace BEngineEditor
{
	public class Scene : AssetData
	{
		public string SceneName { get; set; } = "New Scene";
		public List<SceneEntity> Entities { get; set; } = new();

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
