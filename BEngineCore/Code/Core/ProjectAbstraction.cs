
namespace BEngineCore
{
	public class ProjectAbstraction
	{
		public static ProjectAbstraction LoadedProject { get; private set; }

		protected Scripting scripting = new();
		protected AssetReader reader;
		protected Logger logger = new(true);
		protected Scene loadedScene;

		public Scripting Scripting => scripting;
		public AssetReader AssetsReader => reader;
		public Logger Logger => logger;
		public Scene LoadedScene => loadedScene;

		public void LoadProject()
		{
			LoadedProject = this;
			LoadProjectData();
		}

		public virtual void LoadProjectData()
		{

		}

		public Scene? TryLoadScene(string metaID, bool fastLoad = false)
		{
			Scene? scene = AssetData.Load<Scene>(metaID, this);

			if (scene != null)
			{
				loadedScene?.Save<Scene>();
				OnScenePreLoaded(scene);
				if (fastLoad)
				{
					loadedScene = scene;
					loadedScene.LoadScene();
				}
			}

			return scene;
		}

		public virtual void OnScenePreLoaded(Scene scene)
		{

		}
	}
}
