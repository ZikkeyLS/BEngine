
namespace BEngineEditor
{
	[Serializable]
	public class Scene
	{
		private string _guid = string.Empty;

		public string SceneName = string.Empty;
		// Scene GameObjectsReferences


		public Scene()
		{

		}

		public Scene(string guid)
		{
			_guid = guid;
		}
	}
}
